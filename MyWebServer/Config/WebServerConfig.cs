using System;
using System.Xml.Linq;
using Resouces;
using Host;
using Host.Session;
using Host.Users;
using System.Linq;

namespace Config
{
	public class WebServerConfig {


		public RedirectConfig RedirectConfigure;
        public IItem ResourceLinker;
		public WebSerwer Host;
		public SessionCollect Collector;
		public UserBank Users;

        public WebServerConfig() {}

        private XElement GetElements(IConfigurate conf) {
            if (conf.ConfigName.Length > 1) {
                XElement data = new XElement("data");
                data.Add(
                    (from el in Repository.ConfigBody.Elements()
                     where is_have_name(el.Name.ToString(), conf)
                     select el)
                );
                return data;
            }
            else {
                return Repository.ConfigBody.Element(conf.ConfigName[0]);
            }
        }

        private bool is_have_name(string name, IConfigurate conf) {
            for (int i = 0; i < conf.ConfigName.Length; i++) {
                if (name == conf.ConfigName[i]) { return true; }
            }
            return false;
        }

		public void Configurate() {
            Users = new UserBank();
            Users.Configurate(GetElements(Users));

            LinkDirectory d = new LinkDirectory();
            ResourceLinker = d;
            d.Configurate(GetElements(d));

            RedirectConfigure = new RedirectConfig();
			RedirectConfigure.Configurate(GetElements(RedirectConfigure));

			Collector = new SessionCollect();
			Collector.Configurate(GetElements(Collector));

			Host = new WebSerwer();
			Host.Configurate(GetElements(Host));
		}
	}
}
