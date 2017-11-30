using Host.MIME;
using Host;

namespace MIMEHandlers
{
	public class CSSIMIME : IMIME
	{
		public string MIME_Type { get { return "text/css"; } }
		public string file_extension { get { return ".css"; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			return read.data;
		}
	}
}
