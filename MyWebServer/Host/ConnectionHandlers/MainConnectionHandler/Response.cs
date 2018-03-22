using System;
using Host.ServerExceptions;
using Host.MIME;
using System.Collections.Generic;
using System.Text;
using Config;
using System.Linq;
using Host.Session;
using System.Net.Sockets;
using System.IO;

namespace Host.ConnectionHandlers
{
	public enum AddMode {
		adding,
		rewrite
	}

	public class ResponseDataStream : Stream {
		private Action<byte[]> AddingData;

		public ResponseDataStream(Action<byte[]> AddingData) {
			this.AddingData = AddingData;
		}

		public override bool CanRead { get { return false; } }

		public override bool CanSeek { get { return false; } }

		public override bool CanWrite { get { return true; } }

		public override long Length { get { throw new NotImplementedException(); } }

		public override long Position {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public override void Flush() { throw new NotImplementedException(); }

		public override int Read(byte[] buffer, int offset, int count) { throw new NotImplementedException(); }

		public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }

		public override void SetLength(long value) { throw new NotImplementedException(); }

		public override void Write(byte[] buffer, int offset, int count) {
			AddingData(buffer.Skip(offset).ToArray());
		}
	}

	public class Response
    {
//HTTP/1.1 200 OK\r\n
//Server: MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)\r\n
//Content-Length: {content_length}\r\n
//Connection: close\r\n
//Content-Type: text/html; charset=UTF-8\r\n\r\n
//the content of which length is equal to { content_length }

//        private string bolvanka = @"HTTP/1.1 {0}
//Server: MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)
//Content-Length: {1}
//Connection: close
//Content-Type: {2}; charset=UTF-8
//
//";
		public UserConnect UserData;
		public ResponseDataStream DataWriter;
        private string bolvanka = "HTTP/1.1 {0}\r\nServer: MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux){1}\r\n\r\n";
        public ExceptionCode code;
		private Dictionary<string, string> http_headers;
		private Dictionary<string, string> http_cookie;
		private List<byte[]> http_body;
		private TcpClient Connection;

        private bool IsFatal {
            get { return code.IsFatal; }
        }

        public Response(TcpClient connection) {
			http_headers = new Dictionary<string, string>();
			http_cookie = new Dictionary<string, string>();
			http_body = new List<byte[]>();
			Connection = connection;
			DataWriter = new ResponseDataStream((obj) => http_body.Add(obj));
        }
		public void Clear() {
			http_headers.Clear();
			http_cookie.Clear();
			http_body.Clear();
			code = null;
		}

		public void AddToHeader(string _key, string _value, AddMode mode) {
			try {
                http_headers.Add(_key, _value);
            }
            catch (Exception is_has) {
				if (mode == AddMode.rewrite) {
					http_headers[_key] = _value;
				}
				else {
					throw is_has;
				}
            }
		}

		public void AddToBody(string data) {
			http_body.Add(Encoding.UTF8.GetBytes(data));
		}
		public void AddToBody(byte[] data) {
			http_body.Add(data);
		}

		public void SetCookie(string name, string value, params string[] settings) {
			//Set-Cookie: name=value
			string adding_cookie = string.Format("={0}", value);
			for (int i = 0; i < settings.Length; i++) {
				adding_cookie += "; "+settings[i];
			}
			http_cookie.Add(name, adding_cookie);
		}

		public bool SendData(Reqest request) { //false on connection close
			if (code.IsFatal) {
				http_body.Clear();
				http_headers.Clear();
				http_cookie.Clear();
			}
			Response response = this;
			code.ExceptionHandle(ref request, ref response);

			long data_length = 0;
			foreach (byte[] arr in http_body) {
				data_length += arr.Length;
			}

			AddToHeader("Content-Length", data_length.ToString(), AddMode.rewrite);
			bool keep_alive = true;
			try {
				if (request.preferens["Connection"] == "close") {
					keep_alive = false;
				}
			}
			catch (Exception err) {
				keep_alive = false;
			}
			if (keep_alive) {
				try {
					AddToHeader("Connection", "keep-alive", AddMode.adding);
				}
				catch (Exception err) {
					keep_alive = false;
				}
			}

            string httpbody = "";
            foreach (KeyValuePair<string, string> word in http_headers) {
                httpbody += "\r\n" + word.Key + ": " + word.Value;
            }
			foreach (KeyValuePair<string, string> vord in http_cookie) {
                httpbody += "\r\nSet-Cookie: " + vord.Key + vord.Value;
            }
            string req_header_string = string.Format(bolvanka, code.GetExeptionCode(), httpbody);
			byte[] header = Encoding.UTF8.GetBytes(req_header_string);
			Connection.GetStream().Write(header, 0, header.Length);
			foreach (byte[] arr in http_body) {
				Connection.GetStream().Write(arr, 0, arr.Length);
			}

			if (!keep_alive) {
				Connection.Close();
			}
			return keep_alive;

        }
    }
}
