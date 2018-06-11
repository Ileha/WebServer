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

	public class Response : IDisposable
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
        private string bolvanka = "HTTP/1.1 {0}{1}\r\n\r\n";
        public ExceptionCode code { get; private set; }
		private Dictionary<string, string> http_headers;
		private Dictionary<string, string> http_cookie;
		private List<string> forbidden_http_headers;
        private MemoryStream OutData;
        private NetworkStream _writable_data;

        public static Response Create(TcpClient client, out Func<Stream> Input) {
            Response res = new Response(client);
            Input = () => res.OutData;
            return res;
        }

        public Response(TcpClient client) {
			http_headers = new Dictionary<string, string>();
			http_cookie = new Dictionary<string, string>();
			forbidden_http_headers = new List<string>();
            OutData = new MemoryStream();
            _writable_data = client.GetStream();
            code = Repository.ExceptionFabrics["OK"].Create(null, null);
        }

        public void Clear() {
            http_headers.Clear();
            http_cookie.Clear();
            if (OutData.Length != 0) { 
                OutData.Dispose();
                OutData = new MemoryStream();
            }
            forbidden_http_headers.Clear();
        }

        public void SetCode(ExceptionCode new_code) {
            code = new_code;
        }

		public void AddForbiddenHeader(string header_title) {
			forbidden_http_headers.Add(header_title);
		}

		public string GetHeader(string _key) {
			return http_headers[_key];
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

		public void SetCookie(string name, string value, params string[] settings) {
			//Set-Cookie: name=value
            string adding_cookie = value;
			for (int i = 0; i < settings.Length; i++) {
				adding_cookie += "; "+settings[i];
			}
			http_cookie.Add(name, adding_cookie);
		}

		public void SendData(Reqest request) {
			AddToHeader("Server", "MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)", AddMode.rewrite);
			long data_length = OutData.Length;
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
					if (http_headers["Connection"] == "close") {
						keep_alive = false;
					}
				}
			}
            else {
                AddToHeader("Connection", "close", AddMode.rewrite);
            }

            StringBuilder httpbody = new StringBuilder();
            foreach (KeyValuePair<string, string> word in http_headers) {
				if (!forbidden_http_headers.Contains(word.Key)) {
                    httpbody.AppendFormat("\r\n{0}: {1}", word.Key, word.Value);
				}
            }
			foreach (KeyValuePair<string, string> word in http_cookie) {
                httpbody.AppendFormat("\r\nSet-Cookie: {0}={1}", word.Key, word.Value);
            }
            string req_header_string = string.Format(bolvanka, code.GetExeptionCode(), httpbody.ToString());
			byte[] header = Encoding.UTF8.GetBytes(req_header_string);
			_writable_data.Write(header, 0, header.Length);
            OutData.Seek(0, SeekOrigin.Begin);
            OutData.CopyTo(_writable_data);
        }

        public void Dispose()
        {
            OutData.Dispose();
        }
    }
}
