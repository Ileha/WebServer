using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;
using System;

namespace DataHandlers {
    public abstract class ABSMIME {
        public abstract string[] file_extensions { get; }
        //AddMode rewrite in default
        public abstract void Handle(IConnetion Connection, Action<string, string> add_to_http_header_request);
    }
}
