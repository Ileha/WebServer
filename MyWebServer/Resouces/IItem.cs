using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Resouces
{
    public interface IItem {
        void AddItem(IItem adder_item);
        string GetName();
        void Remove(IItem rem_item);
        IItem GetParent();
        FileSystemInfo GetInfo();
        IItem GetResourceByString(string path);
        IItem Element(string name);
    }
}
