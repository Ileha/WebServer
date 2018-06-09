using System;
using Config;
using Host.ConnectionHandlers;

namespace Host.MIME {

    public abstract class ABSMIME {
        public abstract string[] file_extensions { get; }

        public abstract void Headers(ref Response response, ref Reqest reqest, ref Reader read);
		public abstract void Handle(ref IConnetion connection);
    }
}
