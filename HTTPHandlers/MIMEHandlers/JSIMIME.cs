using Host.MIME;
using Host;
using Host.ConnectionHandlers;

namespace MIMEHandlers
{
	public class JSIMIME : IMIME
	{
		private string[] _file_extensions = { ".js" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/javascript", AddMode.rewrite);
			response.AddToBody(read.data);   
		}
	}
}
