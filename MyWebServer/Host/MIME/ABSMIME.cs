using System;
using Config;
using Host.ConnectionHandlers;

namespace Host.MIME {

    public abstract class ABSMIME {
        public abstract string[] file_extensions { get; }

        public abstract void Headers(Response response, Reqest reqest, Reader read);
		public abstract void Handle(IConnetion connection);
    }
}
