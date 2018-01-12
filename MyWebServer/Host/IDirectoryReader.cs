using System.IO;
using Resouces;
using Host.DataHeaderInterfaces;
using Config;

namespace Host.DirReader
{
    public interface IDirectoryReader : IConfigurate
    {
        string ItemPars(IItem file);
    }
}
