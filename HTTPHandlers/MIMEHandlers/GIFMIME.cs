using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.MIME;
using Host;

namespace HTTPHandlers
{
    class GIFMIME : IMIME
    {
		private string[] _file_extensions = { ".gif" };
        public string[] file_extensions { get { return _file_extensions; } }

        public void Handle(ref Response response, ref Reqest reqest, ref Reader read)
        {
			response.AddToHeader("Content-Type", "image/gif");
			response.AddToBody(read.data);
        }
    }
}
