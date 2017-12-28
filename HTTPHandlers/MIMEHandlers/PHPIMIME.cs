using Host.MIME;
using Host;
namespace MIMEHandlers
{
	public class PHPIMIME : IMIME
	{
		private string[] _file_extensions = { ".php" };
		public string MIME_Type { get { return "text/php"; } }
		public string[] file_extensions { get { return _file_extensions; } }

		public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			return read.data;
		}
	}
}
