using System;
using MyWebServer.WebServerConfigure;

namespace MyWebServer.MIME {
    public interface IMIME {
        string MIME_Type { get; }
        string file_extension { get; }

        Action<Response, Reqest, Reader, IConfigRead> handle { get; }
    }
}
