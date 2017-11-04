using System;
using MyWebServer.HttpHandler;
using MyWebServer.ServerExceptions;
using MyWebServer.MIME;
using System.Collections.Generic;
using System.Text;
using MyWebServer.WebServerConfigure;

namespace MyWebServer
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

        public bool IsBad {
            get { return code.IsFatal; }
        }

        public Response(ExceptionCode code) {
            this.code = code;
            data = new List<byte>();
        }

        public byte[] GetData(Func<string, IMIME> MimeHandler, Reqest _reqest, Reader _read, IConfigRead config) {
            string MIMEType = "";
            data.Clear();
            if (IsBad) {
                data.AddRange(GetExceptionData());
                MIMEType = "text/html";
            }
            else {
                try {
                    IMIME dataHandle = MimeHandler(_read.file_extension);
                    MIMEType = dataHandle.MIME_Type;
                    dataHandle.handle(this, _reqest, _read, config);//here may be execute anything code
                }
                catch (Exception err) {
                    if (err is ExceptionCode) {
                        code = err as ExceptionCode;
                    }
                    else {
                        code = ExceptionCode.InternalServerError();
                    }
                    Console.WriteLine(err.ToString());
                    return GetData(MimeHandler, _reqest, _read, config);
                }
            }
            data.InsertRange(0, Encoding.UTF8.GetBytes(string.Format(bolvanka, code.ToString(), data.Count.ToString(), MIMEType)));
            Console.WriteLine(Encoding.UTF8.GetString(data.ToArray()));
            return data.ToArray();
        }

        private byte[] GetExceptionData() {
            return Encoding.UTF8.GetBytes("<html><body><h2>An error has occurred code of error " + code.ToString() + "</h2></body></html>");
        }
    }
}
