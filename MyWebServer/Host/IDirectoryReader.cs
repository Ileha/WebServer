using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Host.DirReader
{
    interface IDirectoryReader
    {
        string FilePars(FileInfo file);
        string DirPars(DirectoryInfo sub_dir);
    }
}
