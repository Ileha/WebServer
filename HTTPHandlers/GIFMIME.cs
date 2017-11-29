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
        public string MIME_Type { get { return "image/gif"; } }
        public string file_extension { get { return ".gif"; } }

        public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read)
        {
            return read.data;
        }
    }
}
