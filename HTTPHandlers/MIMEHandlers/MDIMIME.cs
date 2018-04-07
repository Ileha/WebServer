using System;
using Host;
using Host.MIME;
using Host.ConnectionHandlers;

namespace MIMEHandlers
{
	public class MDIMIME : IMIME
	{
		private string[] _file_extensions = { ".md", ".MD" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
		}

		public void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/markdown", AddMode.rewrite);
		}
	}
}
