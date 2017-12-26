using System;
using Host.HttpHandler;
using Host.ServerExceptions;
using Host.MIME;
using System.Collections.Generic;
using System.Text;
using Config;
using Host.Session;

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
        private string bolvanka = "HTTP/1.1 {0}\r\n{1}\r\n";
        private ExceptionCode code;
		private Dictionary<string, string> http_body;

        private bool IsFatal {
            get { return code.IsFatal; }
        }

        public Response(ExceptionCode code) {
            this.code = code;
			http_body = new Dictionary<string, string>();
        }

		public void SetCookie(string name, string value) {
			//Set-Cookie: name=valuee
			http_body.Add("Set-Cookie", string.Format("{0}={1}",name,value));
		}

        public byte[] GetData(Reqest _reqest, Reader _read) {
			http_body.Clear();
            http_body.Add("Server", "MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)");
            http_body.Add("Connection", "close");
            List<byte> data = new List<byte>();
            try {
                if (IsFatal) {
                    data.AddRange(code.GetByteData());
                }
                else {
                    IMIME dataHandle = Repository.DataHandlers[_read.file_extension];
                    http_body.Add("Content-Type", dataHandle.MIME_Type + "; charset=UTF-8");
                    Response resp = this;
                    
                    try {
                        string guid = _reqest.cookies["id"];
                    }
                    catch(KeyNotFoundException err) {}
                    data.AddRange(dataHandle.Handle(ref resp, ref _reqest, ref _read));//here may be execute anything codee
					//SetCookie("test", "code");
                }
            }
            catch (Exception err) {
                if (err.GetType().IsSubclassOf(typeof(ExceptionCode))) {
                    code = err as ExceptionCode;
                }
                else {
                    code = new InternalServerError();
                }
                //Console.WriteLine(err.ToString());
                return GetData(_reqest, _read);
            }

            try {
                code.GetAddingToHeader((key, value) => {
                    try {
                        http_body.Add(key, value);
                    }
                    catch (ArgumentException is_has) {
                        if (IsFatal) {
                            http_body[key] = value;
                        }
                    }
                });
            }
            catch (Exception err) {}
            try {
                http_body.Add("Content-Length", data.Count.ToString());
            }
            catch (ArgumentException is_has) {
                http_body["Content-Length"] = data.Count.ToString();
            }
            string httpbody = "";
            foreach (KeyValuePair<string, string> vord in http_body) {
                httpbody += vord.Key + ": " + vord.Value + "\r\n";
            }
			//Console.WriteLine(httpbody);
            string req_header_string = string.Format(bolvanka, code.GetExeptionCode(), httpbody);
            //Console.WriteLine(req_header_string);
            data.InsertRange(0, Encoding.UTF8.GetBytes(req_header_string));
            return data.ToArray();
        }
    }
}
