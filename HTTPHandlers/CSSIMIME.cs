using Host.MIME;
using Host;

namespace MIMEHandlers
{
	public class CSSIMIME : IMIME
	{
		private string[] _file_extensions = { ".css" };
		public string MIME_Type { get { return "text/css"; } }
		public string[] file_extensions { get { return _file_extensions; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			return read.data;
		}
	}
}
