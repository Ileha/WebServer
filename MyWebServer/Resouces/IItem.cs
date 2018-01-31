using System.IO;
using System.Collections;

namespace Resouces
{
    public abstract class IItem : IEnumerable
    {
        public abstract void AddItem(IItem adder_item);
        public abstract string GetName();
		public abstract void Remove(IItem rem_item);
		public abstract IItem GetParent();
		public abstract FileSystemInfo GetInfo();
		public abstract void SetInfo(FileSystemInfo target, IItem New_parent);
		public abstract IItem GetResourceByString(string path);
		public abstract IItem Element(string name);
		public abstract string GetPath();
		public abstract IEnumerator GetEnumerator();
	}
}
