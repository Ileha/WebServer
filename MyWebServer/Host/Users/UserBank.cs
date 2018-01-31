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

		public string ConfigName {
			get { return "users"; }
		}

		public void Configurate(XElement data) {
			foreach (XElement el in data.Elements()) {
				string name = el.Attribute("name").Value;
				string[] groups = Regex.Split(el.Attribute("groups").Value, ",");
				users.Add(name, new UserInfo(name, el.Attribute("passwd").Value, groups));
			}
		}
	}
}
