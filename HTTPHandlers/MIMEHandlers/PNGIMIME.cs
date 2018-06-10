using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.MIME;
using Host;
using Host.ConnectionHandlers;
using System.IO;

namespace MIMEHandlers
{
    class PNGIMIME : ABSMIME
    {
		private string[] _file_extensions = { ".png", ".PNG" };
        public override string[] file_extensions { get { return _file_extensions; } }

		public override void Handle(IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
		}

		public override void Headers(Response response, Reqest reqest, Reader read) {
			response.AddToHeader("Content-Type", "image/png", AddMode.rewrite);
		}
	}
}
