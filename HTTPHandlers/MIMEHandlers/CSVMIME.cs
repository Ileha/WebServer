using System;
using Host;
using Host.MIME;
using Host.ConnectionHandlers;

namespace HTTPHandlers
{
	public class CSVMIME : IMIME
	{
		private string[] _file_extensions = { ".csv" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
		}

		public void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/csv", AddMode.rewrite);
		}
	}
}
