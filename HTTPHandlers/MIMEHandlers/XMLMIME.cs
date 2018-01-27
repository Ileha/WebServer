using System;
using Host.MIME;
using Host;

namespace MIMEHandlers
{
	public class XMLMIME : IMIME
	{
		private string[] _file_extensions = { ".xml" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read)
		{
			response.AddToHeader("Content-Type", "text/xml");
			response.AddToBody(read.data);
		}
	}
}
