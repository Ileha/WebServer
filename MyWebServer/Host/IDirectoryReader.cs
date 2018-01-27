using System.IO;
using Resouces;
using Config;

namespace Host.DirReader
{
    public interface IDirectoryReader : IConfigurate
    {
        string ItemPars(IItem file);
    }
}
