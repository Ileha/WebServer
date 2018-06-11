using System;
using Config;
using Host.ConnectionHandlers;
using System.IO;

namespace Host.MIME
{
    public class HtmlMIME : ABSMIME {
		private string[] _file_extensions = { ".html" };
        public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, out Action<Response, Reqest> Headers)
        {
            Connection.ReadData.Data.CopyTo(Connection.OutputData);
            Headers = (response, reqest) =>
            {
                response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
            };
		}
	}
}