using System;

namespace MyWebServer.MIME {
    public interface IMIME {
        string MIME_Type { get; }
        string file_extension { get; }

        Action<Response, Reqest, Reader> handle { get; }
    }
}
