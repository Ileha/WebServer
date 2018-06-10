using Host.MIME;
using Host;
using Host.ConnectionHandlers;
using System;
using System.IO;

namespace MIMEHandlers
{
	public class JSIMIME : ABSMIME
	{
		private string[] _file_extensions = { ".js" };
		public override string[] file_extensions { get { return _file_extensions; } }

		public override void Handle(IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
		}

		public override void Headers(Response response, Reqest reqest, Reader read) {
			response.AddToHeader("Content-Type", "text/javascript", AddMode.rewrite);
		}
	}
}
