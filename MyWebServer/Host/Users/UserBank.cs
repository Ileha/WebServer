using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Config;

namespace Host.Users
{
	public class UserBank : IConfigurate
	{
		public Dictionary<string, UserInfo> users;
		public readonly UserInfo DefaultUser;

		public UserBank() {
			users = new Dictionary<string, UserInfo>();
			DefaultUser = new UserInfo("Default", "");
		}

		public string ConfigName {
			get {
				return "users";
			}
		}

		public void Configurate(XElement data) {
			
		}
	}
}
