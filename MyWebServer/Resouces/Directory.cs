using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resouces
{
    public class LinkDirectory : IItem
    {
        public Dictionary<string, IItem> contain;
        public DirectoryInfo Resource;
        public IItem Parent;
        private DirectoryInfo directoryInfo;

        public LinkDirectory(DirectoryInfo inf, IItem parent)
        {
            contain = new Dictionary<string, IItem>();
            Resource = inf;
            Parent = parent;
            foreach (DirectoryInfo d in inf.GetDirectories())
            {
                AddItem(new LinkDirectory(d, this));
            }
            foreach (FileInfo f in inf.GetFiles())
            {
                AddItem(new LinkFile(f, this));
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
            return Parent;
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
                else if (bit == ".") {}
                else if (bit == "..") {
                    result = Parent;
                }
            }
            return result;
        }

        public IItem Element(string name)
        {
            return contain[name];
        }

        public string GetPath()
        {
            IItem i = this;
            string res = "";

            while (i.GetParent() != null)
            {
                res = "/" + i.GetName() + res;
                i = i.GetParent();
            }
            return res;
        }


        public System.Collections.IEnumerator GetEnumerator()
        {
            return contain.Values.GetEnumerator();
        }
    }
}
