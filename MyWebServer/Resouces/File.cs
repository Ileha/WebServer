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

        public override string GetName()
        {
            return Resource.Name;
        }

        public override IItem GetParent()
        {
            return Parent;
        }

        public override FileSystemInfo GetInfo()
        {
            return Resource;
        }

        public override void AddItem(IItem adder_item)
        {
            throw new NotImplementedException();
        }

        public override void Remove(IItem rem_item)
        {
            throw new NotImplementedException();
        }

        public override IItem GetResourceByString(string path)
        {
            throw new NotImplementedException();
        }

        public override IItem Element(string name)
        {
            throw new NotImplementedException();
        }
        public override string GetPath()
        {
            IItem i = this;
            string res = "";
            while (i.GetParent() != null) {
                res = "/" + i.GetName() + res;
                i = i.GetParent();
            }
            return res;
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }


        public override void SetInfo(FileSystemInfo target, IItem New_parent)
        {
            if (target is FileInfo)
            {
                Resource = target as FileInfo;
                Parent = New_parent;
            }
            else
            {
                throw new FormatException(target.FullName);
            }
        }
    }
}
