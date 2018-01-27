using Host.MIME;
using Host;
namespace MIMEHandlers
{
	public class PHPIMIME : IMIME
	{
		private string[] _file_extensions = { ".php" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/php");
			response.AddToBody(read.data);
		}
	}
}
