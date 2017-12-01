using System;
using Config;
using Host.MIME;

namespace Host.MIME
{
    public class HttpMIME : IMIME {
		private string[] _file_extensions = { ".html" };
        public string MIME_Type { get { return "text/html"; } }
        public string[] file_extensions { get { return _file_extensions; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			return read.data;
		}
	}
}