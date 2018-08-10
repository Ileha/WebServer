using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DataHandlers;
using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;

namespace HTTPHandlers
{
    class GIFMIME : ABSMIME
    {
		private string[] _file_extensions = { ".gif" };
        public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, out Action<Response, Reqest> Headers)
        {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
            Connection.ReadData.Data.CopyTo(Connection.OutputData);
            Headers = (response, reqest) =>
            {
                response.AddToHeader("Content-Type", "image/gif", AddMode.rewrite);
            };
		}
	}
}
