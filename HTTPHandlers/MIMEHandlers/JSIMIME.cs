using System;
using DataHandlers;
using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;

namespace MIMEHandlers
{
	public class JSIMIME : ABSMIME
	{
		private string[] _file_extensions = { ".js" };
		public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, out Action<Response, Reqest> Headers)
        {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
            Connection.ReadData.Data.CopyTo(Connection.OutputData);
            Headers = (response, reqest) =>
            {
                response.AddToHeader("Content-Type", "text/javascript", AddMode.rewrite);
            };
		}
	}
}
