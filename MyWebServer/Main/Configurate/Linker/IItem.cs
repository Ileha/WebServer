using System.IO;
using System.Collections;
using Configurate.Users;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Configurate.Resouces
{
    public abstract class IItem : IEnumerable
    {
		protected List<GroupInfo> Groups;

		public IItem() {
			Groups = new List<GroupInfo>();
		}

		public bool IsUserEnter(UserInfo user) {
			foreach (GroupInfo first in Groups) {
                if (first.isConsistUser(user)) {
                    return true;
                }
			}
			return false;
		}
		public void ClearAllGroupe() {
			Groups.Clear();
		}
		public void AddGroupe(GroupInfo groupe_name) {
			Groups.Add(groupe_name);
		}
        public void AddGroupe(GroupInfo[] groupes_name) {
            for (int i = 0; i < groupes_name.Length; i++) {
                Groups.Add(groupes_name[i]);
            }
        }

		private IItem _parent;
		public IItem Parent { 
			get { return _parent; }
			set { _parent = value; }
		}
		public string GetPath() {
			IItem i = this;
            if (i.Parent == null) {
                return "/";
            }
            string res = "";

			while (i.Parent != null) {
                res = "/" + i.GetName() + res;
                i = i.Parent;
            }
            return res;
		}

		public abstract string GetName();
		public abstract string Extension { get; }
		public abstract void RemoveThis();
		//public abstract FileSystemInfo GetInfo();

		public virtual Stream GetData() { throw new NotImplementedException(); }
		public virtual void SetInfo(FileSystemInfo target, IItem New_parent) { throw new NotImplementedException(); }
		public virtual void SetInfo(byte[] data, IItem New_parent, string _name) { throw new NotImplementedException(); }
		public virtual void SetInfo(byte[] data, IItem New_parent, string _name, string _extension) { throw new NotImplementedException(); }
		public virtual void AddItem(IItem adder_item) { throw new NotImplementedException(); }
        public virtual void Remove(IItem rem_item) { throw new NotImplementedException(); }
        public virtual IItem GetResourceByString(string path) { throw new NotImplementedException(); }
        public virtual IItem Element(string name) { throw new NotImplementedException(); }
        public virtual IEnumerator GetEnumerator() { throw new NotImplementedException(); }
	}
}
