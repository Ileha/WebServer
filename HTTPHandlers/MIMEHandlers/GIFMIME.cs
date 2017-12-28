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
        public string MIME_Type { get { return "image/gif"; } }
        public string[] file_extensions { get { return _file_extensions; } }

        public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read)
        {
            return read.data;
        }
    }
}
