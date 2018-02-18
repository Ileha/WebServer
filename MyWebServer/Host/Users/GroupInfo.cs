using System;
using System.Collections.Generic;

namespace Host.Users
{
	public class GroupInfo {
		private string name;
		public string Name {
			get { return name; }
		}

        private Dictionary<string, UserInfo> Contain;

		public GroupInfo(string name, params UserInfo[] groups) {
            this.name = name;
            Contain = new Dictionary<string, UserInfo>();
            for (int i = 0; i < groups.Length; i++) {
                AddUser(groups[i]);
            }
		}

        public void AddUser(UserInfo user) {
            Contain.Add(user.Name, user);
        }

        public void RemoveUser(string name) {
            Contain.Remove(name);
        }
        public void RemoveUser(UserInfo user)
        {
            RemoveUser(user.Name);
        }

        public bool isConsistUser(string user_name) {
            return Contain.ContainsKey(user_name);
        }
        public bool isConsistUser(UserInfo user)
        {
            return isConsistUser(user.Name);
        }
	}
}
