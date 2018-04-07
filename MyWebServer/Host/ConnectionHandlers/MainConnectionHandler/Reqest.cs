using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Host.HttpHandler;
using Host.ServerExceptions;
using System.Net.Sockets;
using System.IO;

namespace Host.ConnectionHandlers {
    public class ReqestDataStream : Stream
    {
        private byte[] _data;
        private long _lenght;
        private long _index;
        private Stream _client;

        public ReqestDataStream(byte[] data, long lenght_client, Stream client) {
            _index = 0;
            _data = data;
			if (data != null) {
				_lenght = lenght_client+data.Length;
			}
			else {
				_lenght = lenght_client;
			}
            _client = client;
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override long Length { get { return _lenght; } }
        public override long Position { get { return _index; } set { throw new NotImplementedException(); } }
        public override void Flush() { throw new NotImplementedException(); }

		public override int Read(byte[] buffer, int offset, int count) {
			int res = 0;
            if (_data != null && _index < _data.Length && _index + count <= _data.Length) {
                res = WriteFromData(buffer, offset, _index, count);
            }
            else if (_data != null && _index < _data.Length && _index + count > _data.Length) {
                res = WriteFromData(buffer, offset, _index, _data.Length - _index);
				res += _client.Read(buffer, (int)(_data.Length-_index), (int)(count-(_data.Length - _index)));
            }
            else if (_data == null || _index > _data.Length) {
                res = _client.Read(buffer, offset, count);
            }
			_index += res;
			return res;
        }

        private int WriteFromData(byte[] buffer, int start_buffer, long start, long count) {
			int res = 0;
            for (long i = start; i < start + count; i++) {
                buffer[start_buffer] = _data[i];
                start_buffer++;
				res++;
				if (i == _data.Length - 1) { break; }
            }
			return res;
        }

        public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
        public override void SetLength(long value) { throw new NotImplementedException(); }
        public override void Write(byte[] buffer, int offset, int count){ throw new NotImplementedException(); }
    }

    public class Reqest {
        private static byte[] new_line = new byte[] { 13, 10, 13, 10 };

        public string URL;
        public ReqestDataStream Data;
        public Dictionary<string, string> varibles;//данные
        public Dictionary<string, string> preferens;//заголовки
		public Dictionary<string, string> cookies;

        public Reqest(Stream client) {
            varibles = new Dictionary<string, string>();
            preferens = new Dictionary<string, string>();
			cookies = new Dictionary<string, string>();

			List<byte> input_data = new List<byte>();
			byte[] buffer = new byte[1024];
			int count = 0;
			int index = 0;
			while ((count = client.Read(buffer, 0, buffer.Length)) > 0) {
				input_data.AddRange(buffer.Take(count));
				if (ExistSeqeunce(new_line, buffer, out index)) { //Запрос обрывается \r\n\r\n последовательностью
					index += (input_data.Count - count);
					break;
				}
			}

			string headers_data = Encoding.UTF8.GetString(input_data.GetRange(0, index).ToArray());
			byte[] data = null;
			if (index + 4 < input_data.Count) {
				data = input_data.GetRange(index + 4, input_data.Count - (index + 4)).ToArray();
			}
			Reqest result = this;
			string[] elements = Regex.Split(headers_data, "\r\n");
			try {
				string[] header = elements[0].Split(' ');
				IHttpHandler _handler = Repository.ReqestsHandlers[header[0] + header[2]];
				_handler.ParseHeaders(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
				if (_handler.CanHasData(this)) {
					Data = new ReqestDataStream(data, _handler.GetDataLenght(this), client);
				}
			}
            catch (Exception err) {
                if (err.GetType().IsSubclassOf(typeof(ExceptionCode))) {
                    throw err;
                }
                else {
					throw Repository.ExceptionFabrics["Bad Request"].Create(null, null);
                }
            }
        }

        public void CheckTabelOfRedirect() {
            string new_url;
            try {
                new_url = Repository.Configurate.RedirectConfigure.GetTargetRedirect(URL);
            }
            catch (Exception err) { 
                return;
            }
			throw Repository.ExceptionFabrics["Moved Permanently"].Create(null, new_url);
        }

        private static bool ExistSeqeunce(byte[] sequence, IEnumerable<byte> array, out int index) {
            int seq_i = 0;
            for (int i = 0; i < array.Count(); i++)
            {
                if (array.ElementAt(i) != sequence[seq_i])
                {
                    i = i - seq_i;
                    seq_i = 0;
                }
                else
                {
                    if (seq_i == sequence.Length - 1)
                    {
                        index = i - seq_i;
                        return true;
                    }
                    seq_i++;
                }
            }
            index = -1;
            return false;
        }

    }
}
