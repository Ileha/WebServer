using Host.MIME;
using Host;
using Host.ConnectionHandlers;
using System.IO;

namespace MIMEHandlers
{
	public class CSSIMIME : ABSMIME
	{
		private string[] _file_extensions = { ".css" };
		public override string[] file_extensions { get { return _file_extensions; } }

        public override void Headers(Response response, Reqest reqest, Reader read) {
            response.AddToHeader("Content-Type", "text/css; charset=UTF-8", AddMode.rewrite);
        }

        public override void Handle(IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
        }
    }
}
