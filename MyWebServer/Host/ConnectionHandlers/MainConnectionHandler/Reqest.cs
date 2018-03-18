using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Host.HttpHandler;
using Host.ServerExceptions;
using System.Net.Sockets;
using Host.DataInput;
using Host.HeaderData;
using System.IO;

namespace Host.ConnectionHandlers {
    public class ReqestDataStream : Stream
    {
        private byte[] _data;
        private long _lenght;
        private long _index;
        private NetworkStream _client;

        public ReqestDataStream(byte[] data, long lenght, NetworkStream client) {
            _index = 0;
            _data = data;
            _lenght = lenght;
            _client = client;
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override long Length { get { return _lenght; } }
        public override long Position { get { return _index; } set { throw new NotImplementedException(); } }
        public override void Flush() { throw new NotImplementedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _index += offset;
            int new_count = count;
            if (_index + count > Length) {
                new_count = (int)(Length - _index);
            }

            if (_data != null && _index < _data.Length && _index + new_count <= _data.Length) {
                WriteFromData(buffer, 0, _index, new_count);
            }
            else if (_data != null && _index < _data.Length && _index + new_count > _data.Length) {
                WriteFromData(buffer, 0, _index, _data.Length - _index);
				WriteFromClient(buffer, _data.Length - _index, new_count - (_data.Length - _index));
            }
            else if (_data == null || _index > _data.Length) {
                WriteFromClient(buffer, 0, new_count);
            }
			return new_count;
        }

        private void WriteFromData(byte[] buffer, int start_buffer, long start, long count) {
            for (long i = start; i < start + count; i++) {
                buffer[start_buffer] = _data[i];
                start_buffer++;
            }
        }

        private void WriteFromClient(byte[] buffer, long start_buffer, long count) {
			if (start_buffer != 0) {
				for (long i = start_buffer; i < start_buffer + count; i++) {
					buffer[i] = (byte)_client.ReadByte();
				}
			}
            else {
                _client.Read(buffer, 0, (int)count);
            }
        }

        public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
        public override void SetLength(long value) { throw new NotImplementedException(); }
        public override void Write(byte[] buffer, int offset, int count){ throw new NotImplementedException(); }
    }

    public class Reqest {
        private static byte[] new_line = new byte[] { 13, 10, 13, 10 };

        public string URL;
        public ReqestDataStream Data;
        public Dictionary<string, ABSReqestData> varibles;//данные
        public Dictionary<string, HeaderValueMain> preferens;//заголовки
		public Dictionary<string, string> cookies;
		public TcpClient client;

        public Reqest(TcpClient client) {
            varibles = new Dictionary<string, ABSReqestData>();
            preferens = new Dictionary<string, HeaderValueMain>();
			cookies = new Dictionary<string, string>();
			this.client = client;
        }

        public void CheckTabelOfRedirect() {
            string new_url;
            try {
                new_url = Repository.Configurate.RedirectConfigure.GetTargetRedirect(URL);
            }
            catch (Exception err) { 
                return;
            }
			throw Repository.ExceptionFabrics["Moved Permanently"].Create(new_url);
        }

		public void Clear() {
			varibles.Clear();
			preferens.Clear();
			cookies.Clear();
			URL = "";
			Data = null;
		}

		public void Create() {
            List<byte> input_data = new List<byte>();
            byte[] buffer = new byte[1024];
			int count = 0;
            int index = 0;
            while ((count = client.GetStream().Read(buffer, 0, buffer.Length)) > 0) {
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
                IHttpHandler _handler = Repository.ReqestsHandlers[header[0]+header[2]];
                _handler.ParseHeaders(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
				if (_handler.CanHasData(this)) {
					Data = new ReqestDataStream(data, _handler.GetDataLenght(this), client.GetStream());
				}
            }
            catch (Exception err) {
                if (err.GetType().IsSubclassOf(typeof(ExceptionCode))) {
                    throw err;
                }
                else {
					throw Repository.ExceptionFabrics["Bad Request"].Create(null);
                }
            }
        }

        public static bool ExistSeqeunce(byte[] sequence, IEnumerable<byte> array, out int index) {
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
