using System.IO;
using System.Collections;
using Host.Users;
using System.Linq;
using System.Collections.Generic;

namespace Resouces
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
        public void AddGroupe(GroupInfo[] groupes_name)
        {
            for (int i = 0; i < groupes_name.Length; i++) {
                Groups.Add(groupes_name[i]);
            }
        }

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
