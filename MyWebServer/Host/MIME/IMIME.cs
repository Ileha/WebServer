using System;
using Config;

namespace Host.MIME {
	//public delegate void GetMessage(ref Response response, ref Reqest reqest, ref Reader read);

    public interface IMIME {
        string MIME_Type { get; }
        string file_extension { get; }

		byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read);
    }
}
