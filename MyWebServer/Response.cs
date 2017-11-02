using System;
using MyWebServer.HttpHandler;
using MyWebServer.ServerExceptions;
using MyWebServer.MIME;
using System.Collections.Generic;
using System.Text;

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

        private string bolvanka = @"{0} {1}
Server: MyWebServer(0.0.0.1) (Unix) (Red-Hat/Linux)
Content-Length: {2}
Connection: close
Content-Type: {3}; charset=UTF-8

";
        private ExceptionCode code;
        private string HttpVersion;
        public readonly List<byte> data;

        public bool IsBad {
            get { return code.IsFatal; }
        }

        public Response(ExceptionCode code, IHttpHandler handler) {
            this.code = code;
            try {
                HttpVersion = handler.Version;
            }
            catch (Exception err) {
                HttpVersion = "HTTP/1.1";
            }
            data = new List<byte>();
        }

        public byte[] GetData(ConnectionHandler connect) {
            string MIMEType = "";
            data.Clear();
            if (IsBad) {
                data.AddRange(GetExceptionData());
                MIMEType = "text/html";
            }
            else {
                try {
                    IMIME dataHandle = connect.GetIMIMEHandler(connect.reads_bytes.file_extension);
                    MIMEType = dataHandle.MIME_Type;
                    dataHandle.handle(this, connect.obj_request, connect.reads_bytes);
                }
                catch (Exception err) {
                    Console.WriteLine(err.ToString());
                    code = ExceptionCode.InternalServerError();
                    return GetData(connect);
                }
            }
            data.InsertRange(0, Encoding.UTF8.GetBytes(string.Format(bolvanka, HttpVersion, code.ToString(), data.Count.ToString(), MIMEType)));
            Console.WriteLine(Encoding.UTF8.GetString(data.ToArray()));
            return data.ToArray();
        }

        private byte[] GetExceptionData() {
            return Encoding.UTF8.GetBytes("<html><body><h2>An error has occurred code of error " + code.ToString() + "</h2></body></html>");
        }
    }
}
