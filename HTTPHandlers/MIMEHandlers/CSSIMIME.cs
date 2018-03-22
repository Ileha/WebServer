using Host.MIME;
using Host;
using Host.ConnectionHandlers;

namespace MIMEHandlers
{
	public class CSSIMIME : IMIME
	{
		private string[] _file_extensions = { ".css" };
		public string[] file_extensions { get { return _file_extensions; } }

        public void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
            response.AddToHeader("Content-Type", "text/css; charset=UTF-8", AddMode.rewrite);
        }

        public void Handle(ref IConnetion connection) {
			connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
        }
    }
}
