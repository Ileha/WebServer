using System;
using System.IO;
using DataHandlers;
using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;

namespace Host.MIME
{
    public class HtmlMIME : ABSMIME {
		private string[] _file_extensions = { ".html" };
        public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, Action<string, string> add_to_http_header_request)
        {
            Connection.ReadData.Data.CopyTo(Connection.OutputData);
            add_to_http_header_request("Content-Type", "text/html; charset=UTF-8");
		}
	}
}