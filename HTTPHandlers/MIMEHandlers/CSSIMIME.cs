using Host.MIME;
using Host;
using Host.ConnectionHandlers;
using System.IO;
using System;

namespace MIMEHandlers
{
	public class CSSIMIME : ABSMIME
	{
		private string[] _file_extensions = { ".css" };
		public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, out Action<Response, Reqest> Headers)
        {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
            Connection.ReadData.Data.CopyTo(Connection.OutputData);
            Headers = (response, reqest) =>
            {
                response.AddToHeader("Content-Type", "text/css; charset=UTF-8", AddMode.rewrite);
            };
        }

    }
}
