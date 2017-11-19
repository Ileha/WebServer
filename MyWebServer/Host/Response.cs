using System;
using Host.HttpHandler;
using Host.ServerExceptions;
using Host.MIME;
using System.Collections.Generic;
using System.Text;
using Config;

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

        private string bolvanka = @"HTTP/1.1 {0}
Server: MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)
Content-Length: {1}
Connection: close
Content-Type: {2}; charset=UTF-8

";
        private ExceptionCode code;
        public readonly List<byte> data;

        public Response(ExceptionCode code) {
            this.code = code;
            data = new List<byte>();
        }

        public byte[] GetData(Reqest _reqest, Reader _read) {
            string MIMEType = "";
            data.Clear();
            if (code.IsFatal) {
                data.AddRange(GetExceptionData());
                MIMEType = "text/html";
            }
            else {
                try {
                    IMIME dataHandle = Repository.DataHandlers[_read.file_extension];
                    MIMEType = dataHandle.MIME_Type;
					Response resp = this;
                    dataHandle.Handle(ref resp, ref _reqest, ref _read);//here may be execute anything code
                }
                catch (Exception err) {
                    if (err is ExceptionCode) {
                        code = err as ExceptionCode;
                    }
                    else {
                        code = new InternalServerError();
                    }
                    Console.WriteLine(err.ToString());
                    return GetData(_reqest, _read);
                }
            }
            data.InsertRange(0, Encoding.UTF8.GetBytes(string.Format(bolvanka, code.GetExeptionCode(), data.Count.ToString(), MIMEType)));
            Console.WriteLine(Encoding.UTF8.GetString(data.ToArray()));
            return data.ToArray();
        }

        private byte[] GetExceptionData() {
			return Encoding.UTF8.GetBytes("<html><body><h2>An error has occurred code of error " + code.GetExeptionCode() + "</h2></body></html>");
        }
    }
}
