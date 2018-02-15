using System.IO;
using Resouces;
using Config;

namespace Host.DirReader
{
    public interface IDirectoryReader
    {
        string ItemPars(IItem file);
    }
}
