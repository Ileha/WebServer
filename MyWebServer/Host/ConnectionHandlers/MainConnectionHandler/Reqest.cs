﻿using System;
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
		private NetworkStream _client;

		public ReqestDataStream(byte[] data, long lenght_client, NetworkStream client)
		{
			_index = 0;
			_data = data;
			_lenght = lenght_client;
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
			try {
				_index += offset;
				for (int i = 0; i < count; i++) {
					_data[_index] = buffer[i];
					_index++;
					res++;
				}
			}
			catch (Exception err) {
				//_client.Read(buffer, )
			}
			
		}

		public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
		public override void SetLength(long value) { throw new NotImplementedException(); }
		public override void Write(byte[] buffer, int offset, int count) { throw new NotImplementedException(); } 
	}

    public class Reqest {
        private static byte[] new_line = new byte[] { 13, 10, 13, 10 };

        public string URL;
        public MemoryStream Data;
        public Dictionary<string, string> varibles;//данные
        public Dictionary<string, string> preferens;//заголовки
		public Dictionary<string, string> cookies;

        public Reqest(TcpClient client) {
            varibles = new Dictionary<string, string>();
            preferens = new Dictionary<string, string>();
			cookies = new Dictionary<string, string>();
			Data = new MemoryStream();

			List<byte> input_data = new List<byte>();
			byte[] buffer = new byte[1024];
			int count = 0;
			int index = 0;
			while ((count = client.Client.Receive(buffer)) > 0) {
				input_data.AddRange(buffer.Take(count));
				if (ExistSeqeunce(new_line, buffer, out index)) { //Запрос обрывается \r\n\r\n последовательностью
					index += (input_data.Count - count);
					break;
				}
			}
			if (count == 0) { throw new ConnectionExecutorClose(); }

			string headers_data = Encoding.UTF8.GetString(input_data.GetRange(0, index).ToArray());
			if (index + 4 < input_data.Count) {
				Data.Write(input_data.GetRange(index + 4, input_data.Count - (index + 4)).ToArray(), 0, input_data.Count - (index + 4));
			}
			Reqest result = this;
			string[] elements = Regex.Split(headers_data, "\r\n");
			try {
				string[] header = elements[0].Split(' ');
				IHttpHandler _handler = Repository.ReqestsHandlers[header[0] + header[2]];
				_handler.ParseHeaders(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
				if (_handler.CanHasData(this) && (int)_handler.GetDataLenght(this) - Data.Length > 0) {
					int lenght_all = (int)_handler.GetDataLenght(this);
					byte[] d = new byte[1024];
					do {
						int l = client.Client.Receive(d);
						Data.Write(d, 0, l);
					} while ((lenght_all - Data.Length) < 0);
					Data.Seek(0, SeekOrigin.Begin);
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
