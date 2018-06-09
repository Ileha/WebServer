using System;
using Host.MIME;
using Host;
using Host.ConnectionHandlers;
using System.IO;

namespace MIMEHandlers
{
	public class XMLMIME : ABSMIME
	{
		private string[] _file_extensions = { ".xml" };
		public override string[] file_extensions { get { return _file_extensions; } }

		public override void Handle(ref IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
            connection.OutputData.Seek(0, SeekOrigin.Begin);
		}

		public override void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/xml", AddMode.rewrite);
		}
	}
}
