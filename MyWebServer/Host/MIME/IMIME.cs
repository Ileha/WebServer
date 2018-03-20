using System;
using Config;
using Host.ConnectionHandlers;

namespace Host.MIME {

    public interface IMIME {
        string[] file_extensions { get; }

		void Handle(ref Response response, ref Reqest reqest, ref Reader read);
    }
}
