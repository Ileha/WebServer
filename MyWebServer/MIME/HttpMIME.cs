using System;
using MyWebServer.HttpHandler;
using MyWebServer.WebServerConfigure;
using MyWebServer.MIME;

namespace MyWebServer.MIME
{
    public class HttpMIME : IMIME {
        public string MIME_Type { get { return "text/html"; } }
        public string file_extension { get { return ".html"; } }

        public Action<Response, Reqest, Reader, IConfigRead> handle {
            get {
                return (resp, req, read, conf) => {
                    resp.data.AddRange(read.data);
                };
            }
        }
    }
}
