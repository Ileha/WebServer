using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.MIME;
using Host;

namespace MIMEHandlers
{
    class JPGMIME : IMIME
    {
		private string[] _file_extensions = { ".jpg", ".JPG" };
        public string[] file_extensions { get { return _file_extensions; } }

        public void Handle(ref Response response, ref Reqest reqest, ref Reader read)
        {
			response.AddToHeader("Content-Type", "image/jpg", AddMode.rewrite);
			response.AddToBody(read.data);
        }
    }
}
