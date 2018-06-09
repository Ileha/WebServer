using System;
using Config;
using Host.ConnectionHandlers;
using System.IO;

namespace Host.MIME
{
    public class HtmlMIME : ABSMIME {
		private string[] _file_extensions = { ".html" };
        public override string[] file_extensions { get { return _file_extensions; } }

		public override void Handle(ref IConnetion connection) {
			connection.ReadData.data.CopyTo(connection.OutputData);
            connection.OutputData.Seek(0, SeekOrigin.Begin);
		}

		public override void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
		}
	}
}