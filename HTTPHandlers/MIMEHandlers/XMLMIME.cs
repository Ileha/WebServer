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

        public override void Handle(IConnetion Connection, out Action<Response, Reqest> Headers)
        {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
            Connection.ReadData.Data.CopyTo(Connection.OutputData);
			
            Headers = (response, reqest) =>
            {
                response.AddToHeader("Content-Type", "text/xml", AddMode.rewrite);
            };
		}
	}
}
