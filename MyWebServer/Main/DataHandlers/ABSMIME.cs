using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;
using System;

namespace DataHandlers {
    public abstract class ABSMIME {
        public abstract string[] file_extensions { get; }

        //public abstract void Headers(Response response, Reqest reqest, IReader read);
        public abstract void Handle(IConnetion Connection, out Action<Response, Reqest> Headers);
    }
}
