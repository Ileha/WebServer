using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.MIME;
using Host;
using Host.ConnectionHandlers;

namespace MIMEHandlers
{
    class JPGMIME : IMIME
    {
		private string[] _file_extensions = { ".jpg", ".JPG" };
        public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
		}

		public void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "image/jpg", AddMode.rewrite);
		}
	}
}
