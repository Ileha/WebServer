using Host.MIME;
using Host;

namespace MIMEHandlers
{
	public class JSIMIME : IMIME
	{
		public string MIME_Type { get { return "text/javascript"; } }
		public string file_extension { get { return ".js"; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			return read.data;   
		}
	}
}
