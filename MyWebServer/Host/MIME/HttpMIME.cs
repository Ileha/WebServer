using System;
using Config;
using Host.MIME;

namespace Host.MIME
{
    public class HttpMIME : MarshalByRefObject, IMIME {
        public string MIME_Type { get { return "text/html"; } }
        public string file_extension { get { return ".html"; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			response.data.AddRange(read.data);
		}
	}
}
