using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Config;
using System.Text.RegularExpressions;

namespace Host.Users
{
	public class UserBank : IConfigurate
	{
		public Dictionary<string, UserInfo> users;

		public UserBank() {
			users = new Dictionary<string, UserInfo>();
		}


        private string[] names = new string[] { "users" };
		public string[] ConfigName {
			get { return names; }
		}

		public void Configurate(XElement data) {
            string[] default_groups = Regex.Split(data.Element("default_user").Value, ",");
			UserInfo all_user = new UserInfo("", "", default_groups);

            foreach (XElement el in data.Element("users").Elements()) {
				string name = el.Attribute("name").Value;
				string[] groups = Regex.Split(el.Attribute("groups").Value, ",");
				users.Add(name, new UserInfo(name, el.Attribute("passwd").Value, groups));
			}
		}
	}
}
