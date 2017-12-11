using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resouces
{
    public class Directory : IItem
    {
        public List<IItem> contain;
        public DirectoryInfo Resource;
        public IItem Parent;
        private DirectoryInfo directoryInfo;

        public Directory(DirectoryInfo inf, IItem parent)
        {
            contain = new List<IItem>();
            Resource = inf;
            Parent = parent;
            foreach (DirectoryInfo d in inf.GetDirectories())
            {
                contain.Add(new Directory(d, this));
            }
            foreach (FileInfo f in inf.GetFiles())
            {
                contain.Add(new File(f, this));
            }
        }

        public void AddItem(IItem adder_item)
        {
            contain.Add(adder_item);
        }

        public string GetName()
        {
            return Resource.Name;
        }

        public IItem GetParent()
        {
            throw new NotImplementedException();
        }

        public FileSystemInfo GetInfo()
        {
            return Resource;
        }

        public void Remove(IItem rem_item)
        {
            contain.Remove(rem_item);
        }
    }
}
