using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.MIME;
using Host;

namespace MIMEHandlers
{
    class PNGIMIME : IMIME
    {
        public string MIME_Type { get { return "image/png"; } }
        public string file_extension { get { return ".png"; } }

        public byte[] Handle(ref Response response, ref Reqest reqest, ref Reader read)
        {
            return read.data;
        }
    }
}
