using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Configurate.Users
{
    public class UserBank : IConfigurate
    {
        private UserInfo _defaultUser;
        public UserInfo DefaultUser {
            get { return _defaultUser; }
        }

        private GroupInfo _defaultGroup;
        public GroupInfo DefaultGroup {
            get { return _defaultGroup; }
        }
        public Dictionary<string, GroupInfo> groups;

		private string[] names = new string[] { "users" };
		public string[] ConfigName {
			get { return names; }
		}

		public UserBank() {
			groups = new Dictionary<string, GroupInfo>();
		}

		public UserInfo GetUserByName(string name) {
			Exception th = null;
			foreach (GroupInfo gr in groups.Values) {
				try {
					return gr.GetUser(name);
				}
				catch (Exception err) {
					th = err;
				}
			}
			throw th;
		}

        public void Configurate(XElement data) {
            string default_group = data.Element("default_user").Attribute("groups").Value;
            _defaultUser = new UserInfo("default", "");
            _defaultGroup = new GroupInfo(default_group, _defaultUser);
            groups.Add(_defaultGroup.Name, _defaultGroup);

            foreach (XElement el in data.Element("users").Elements()) {
                string[] groups_title = Regex.Split(el.Attribute("groups").Value, ",");
                UserInfo new_user = new UserInfo(el.Attribute("name").Value, el.Attribute("passwd").Value);
                

                for (int i = 0; i < groups_title.Length; i++) {
                    GroupInfo curr_group = null;
                    try {
                        curr_group = groups[groups_title[i]];
                    }
                    catch (Exception err) {
                        curr_group = new GroupInfo(groups_title[i]);
                        groups.Add(curr_group.Name, curr_group);
                    }
                    curr_group.AddUser(new_user);
                }
            }
        }
    }
}
