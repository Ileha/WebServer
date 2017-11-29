using System;
using Config;
using Host.MIME;

namespace Host.MIME
{
    public class HttpMIME : IMIME {
        public string MIME_Type { get { return "text/html"; } }
        public string file_extension { get { return ".html"; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			return read.data;
		}
	}
}