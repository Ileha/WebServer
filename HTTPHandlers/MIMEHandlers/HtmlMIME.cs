using System;
using Config;
using Host.ConnectionHandlers;
using System.IO;

namespace Host.MIME
{
    public class HtmlMIME : ABSMIME {
		private string[] _file_extensions = { ".html" };
        public override string[] file_extensions { get { return _file_extensions; } }

		public override void Handle(IConnetion connection) {
			connection.ReadData.data.CopyTo(connection.OutputData);
		}

		public override void Headers(Response response, Reqest reqest, Reader read) {
			response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
		}
	}
}