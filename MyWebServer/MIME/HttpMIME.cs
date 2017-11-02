using System;
using MyWebServer.HttpHandler;

namespace MyWebServer.MIME
{
    public class HttpMIME : IMIME {
        public string MIME_Type { get { return "text/html"; } }
        public string file_extension { get { return ".html"; } }

        public Action<Response, Reqest, Reader> handle {
            get {
                return (resp, req, read) => {
                    resp.data.AddRange(read.data);
                };
            }
        }
    }
}
