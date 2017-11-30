using Host.MIME;
using Host;
namespace MIMEHandlers
{
	public class PHPIMIME : IMIME
	{
		public string MIME_Type { get { return "text/php"; } }
		public string file_extension { get { return ".php"; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			return read.data;
		}
	}
}
