using System;
using Host;
using Host.MIME;
using Host.ConnectionHandlers;
using System.IO;

namespace HTTPHandlers
{
	public class CSVMIME : ABSMIME
	{
		private string[] _file_extensions = { ".csv" };
		public override string[] file_extensions { get { return _file_extensions; } }

		public override void Handle(IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
		}

		public override void Headers(Response response, Reqest reqest, Reader read) {
			response.AddToHeader("Content-Type", "text/csv", AddMode.rewrite);
		}
	}
}
