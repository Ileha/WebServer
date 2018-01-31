using System;

namespace Host.Users
{
	public class UserInfo
	{
		private string name;
		public string Name {
			get { return name; }
		}

		private string[] groups;
		public string[] Groups {
			get { return groups; }
		}

		private string pass;
		public string Password {
			get { return pass; }
		}

		public UserInfo(string name, string pass, params string[] groups) {
			this.name = name;
			this.pass = pass;
			this.groups = groups;
		}
	}
}
