using System;
using Host.HttpHandler;
using Host.ServerExceptions;
using Host.MIME;
using System.Collections.Generic;
using System.Text;
using Config;
using Host.Session;
using System.Net.Sockets;

namespace Host
{
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

        private string bolvanka = "HTTP/1.1 {0}\r\nServer: MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)\r\nConnection: close{1}\r\n\r\n";
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
        }

		public void AddToHeader(string _key, string _value) {
			http_headers.Add(_key, _value);
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
			//AddToHeader("Set-Cookie", adding_cookie);
		}

		public void SendData(Reqest request) {
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

            try {
				AddToHeader("Content-Length", data_length.ToString());
            }
            catch (ArgumentException is_has) {
				http_headers["Content-Length"] = data_length.ToString();
            }

            string httpbody = "";
            foreach (KeyValuePair<string, string> vord in http_headers) {
                httpbody += "\r\n" + vord.Key + ": " + vord.Value;
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
			Connection.Close();
        }
    }
}
