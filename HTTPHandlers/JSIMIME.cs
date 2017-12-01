using Host.MIME;
using Host;

namespace MIMEHandlers
{
	public class JSIMIME : IMIME
	{
		private string[] _file_extensions = { ".js" };
		public string MIME_Type { get { return "text/javascript"; } }
		public string[] file_extensions { get { return _file_extensions; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			return read.data;   
		}
	}
}
