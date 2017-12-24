using System.IO;
using Resouces;
using Host.DataHeaderInterfaces;

namespace Host.DirReader
{
    public interface IDirectoryReader
    {
        string ItemPars(IItem file);
    }
}
