using System.IO;

namespace Host.DirReader
{
    public interface IDirectoryReader
    {
        string FilePars(FileInfo file);
        string DirPars(DirectoryInfo sub_dir);
    }
}
