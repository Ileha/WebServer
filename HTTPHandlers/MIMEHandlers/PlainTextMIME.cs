using System;
using Host;
using Host.MIME;

namespace HTTPHandlers
{
	public class PlainTextMIME : IMIME
	{
		private string[] _file_extensions = { ".txt", ".TXT" };
		public string[] file_extensions { get { return _file_extensions; } }
		public string MIME_Type { get { return "text/plain"; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read)
		{
			return read.data;
		}
	}
}
