using System;
using Host.MIME;
using Host;

namespace MIMEHandlers
{
	public class XMLMIME : IMIME
	{
		private string[] _file_extensions = { ".xml" };
		public string[] file_extensions { get { return _file_extensions; } }
		public string MIME_Type { get { return "text/xml"; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read)
		{
			return read.data;
		}
	}
}
