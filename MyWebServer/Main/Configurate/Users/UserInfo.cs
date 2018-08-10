using System.Collections.Generic;

namespace Configurate.Users
{
	public class UserInfo
	{
		private string name;
		public string Name {
			get { return name; }
		}

		private string pass;
		public string Password {
			get { return pass; }
		}

		public UserInfo(string name, string pass) {
			this.name = name;
			this.pass = pass;
		}
	}
}
