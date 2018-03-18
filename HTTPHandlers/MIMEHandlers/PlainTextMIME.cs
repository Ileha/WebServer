using System;
using Host;
using Host.MIME;

namespace HTTPHandlers
{
	public class PlainTextMIME : IMIME
	{
		private string[] _file_extensions = { ".txt", ".TXT" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read)
		{
			response.AddToHeader("Content-Type", "text/plain; charset=UTF-8", AddMode.rewrite);
			response.AddToBody(read.data);
		}
	}
}
