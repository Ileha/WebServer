using System;
using Host;
using Host.MIME;

namespace MIMEHandlers
{
	public class MDIMIME : IMIME
	{
		private string[] _file_extensions = { ".md", ".MD" };
		public string[] file_extensions { get { return _file_extensions; } }
		public string MIME_Type { get { return "text/markdown"; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read)
		{
			return read.data;
		}
	}
}
