using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resouces
{
    public class LinkFile : IItem
    {
        private FileInfo Resource;
        public IItem Parent;

        public LinkFile(FileInfo inf, IItem _parent) {
            Resource = inf;
            Parent = _parent;
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

        public void AddItem(IItem adder_item)
        {
            throw new NotImplementedException();
        }

        public void Remove(IItem rem_item)
        {
            throw new NotImplementedException();
        }

        public IItem GetResourceByString(string path)
        {
            throw new NotImplementedException();
        }

        public IItem Element(string name)
        {
            throw new NotImplementedException();
        }
        public string GetPath()
        {
            IItem i = this;
            string res = "";
            while (i.GetParent() != null) {
                res = "/" + i.GetName() + res;
                i = i.GetParent();
            }
            return res;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
