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
        private string bolvanka = "HTTP/1.1 {0}{1}\r\n\r\n";
        public ExceptionCode code;
		private Dictionary<string, string> http_headers;
		private Dictionary<string, string> http_cookie;
		private List<byte[]> http_body;
		private List<string> forbidden_http_headers;

        private bool IsFatal {
            get { return code.IsFatal; }
        }

        public Response() {
			http_headers = new Dictionary<string, string>();
			http_cookie = new Dictionary<string, string>();
			http_body = new List<byte[]>();
			forbidden_http_headers = new List<string>();
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

		public void SendData(Reqest request, IConnetion Data, Stream output) {
			if (code.IsFatal) {
				http_body.Clear();
				http_headers.Clear();
				http_cookie.Clear();
			}
			AddToHeader("Server", "MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)", AddMode.rewrite);
			Response response = this;
			code.ExceptionHandle(ref request, ref response, Data);

			long data_length = Data.OutputData.Length;

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
			output.Write(header, 0, header.Length);
			Data.OutputData.CopyTo(output);
        }
    }
}
