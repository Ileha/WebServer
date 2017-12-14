using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Resouces
{
    public interface IItem : IEnumerable
    {
        void AddItem(IItem adder_item);
        string GetName();
        void Remove(IItem rem_item);
        IItem GetParent();
        FileSystemInfo GetInfo();
        void SetInfo(FileSystemInfo target, IItem New_parent);
        IItem GetResourceByString(string path);
        IItem Element(string name);
        string GetPath();
    }
}
