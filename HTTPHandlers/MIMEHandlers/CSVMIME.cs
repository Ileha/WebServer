using System;
using System.IO;
using DataHandlers;
using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;

namespace HTTPHandlers
{
	public class CSVMIME : ABSMIME
	{
		private string[] _file_extensions = { ".csv" };
		public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, Action<string, string> add_to_http_header_request)
        {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
            Connection.ReadData.Data.CopyTo(Connection.OutputData);
            add_to_http_header_request("Content-Type", "text/csv");
		}
	}
}
