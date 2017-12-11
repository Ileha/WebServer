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
        public Dictionary<string, IItem> contain;
        public DirectoryInfo Resource;
        public IItem Parent;
        private DirectoryInfo directoryInfo;

        public Directory(DirectoryInfo inf, IItem parent)
        {
            contain = new Dictionary<string, IItem>();
            Resource = inf;
            Parent = parent;
            foreach (DirectoryInfo d in inf.GetDirectories())
            {
                AddItem(new Directory(d, this));
            }
            foreach (FileInfo f in inf.GetFiles())
            {
                AddItem(new File(f, this));
            }
        }

        public void AddItem(IItem adder_item)
        {
            contain.Add(adder_item.GetName(), adder_item);
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
            contain.Remove(rem_item.GetName());
        }

        public IItem GetResourceByString(string path)
        {
            string[] path_arr = path.Split('/');
            IItem result = this;
            foreach (string bit in path_arr) {
                if (bit != "") {
                    result = result.Element(bit);
                }
            }
            return result;
        }

        public IItem Element(string name)
        {
            return contain[name];
        }
    }
}
