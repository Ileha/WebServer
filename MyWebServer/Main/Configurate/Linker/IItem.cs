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
        public void AddGroupe(params GroupInfo[] groupes_name) {
            for (int i = 0; i < groupes_name.Length; i++) {
                Groups.Add(groupes_name[i]);
            }
        }

        private IitemRead _parent;
        public IitemRead Parent
        { 
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
        public virtual void SetInfo(FileSystemInfo target, IitemRead New_parent) { throw new NotImplementedException(); }
        public virtual void SetInfo(byte[] data, IitemRead New_parent, string _name) { throw new NotImplementedException(); }
        public virtual void SetInfo(byte[] data, IitemRead New_parent, string _name, string _extension) { throw new NotImplementedException(); }
        public virtual void AddItem(IitemRead adder_item) { throw new NotImplementedException(); }
        public virtual void Remove(IitemRead rem_item) { throw new NotImplementedException(); }
        public virtual IitemRead GetResourceByString(string path) { throw new NotImplementedException(); }
        public virtual IitemRead Element(string name) { throw new NotImplementedException(); }
        public virtual IEnumerator GetEnumerator() { throw new NotImplementedException(); }
	}
}
