using Host.MIME;
using Host;

namespace MIMEHandlers
{
	public class CSSIMIME : IMIME
	{
		private string[] _file_extensions = { ".css" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/css; charset=UTF-8", AddMode.rewrite);
			response.AddToBody(read.data);
		}
	}
}
