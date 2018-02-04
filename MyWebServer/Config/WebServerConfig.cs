using System;
using System.Xml.Linq;
using Resouces;
using Host;
using Host.DirReader;
using MainProgramm;
using Host.Session;
using Host.Users;
using System.Linq;

namespace Config
{
	public class WebServerConfig {
        private XElement _body_conf;
		public XElement ConfigBody {
			get {
				return _body_conf;	
			} 
		}

		public RedirectConfig RedirectConfigure;
        public IItem ResourceLinker;
		public WebSerwer Host;
		public IDirectoryReader DirReader;//экземпляр класса преобразующий директорию в html страницу
		public SessionCollect Collector;
		public UserBank Users;

        public WebServerConfig(XElement data, ForAddEvent start) {
			_body_conf = data;
			start(Configurate);
		}

		public XElement this[string index] {
			get {
				try {
					return _body_conf.Element(index);
				}
				catch (Exception err) {
					throw new ErrorServerConfig(index);
				}
			}
		}

        private XElement GetElements(IConfigurate conf) {
            if (conf.ConfigName.Length < 1) {
                XElement data = new XElement("data");
                data.Add(
                    (from el in _body_conf.Elements()
                     where is_have_name(el.Name.ToString(), conf)
                     select el)
                );
                return data;
            }
            else {
                return _body_conf.Element(conf.ConfigName[0]);
            }
        }

        private bool is_have_name(string name, IConfigurate conf) {
            for (int i = 0; i < conf.ConfigName.Length; i++) {
                if (name == conf.ConfigName[i]) { return true; }
            }
            return false;
        }

		private void Configurate() {
            Host = new WebSerwer();
            Host.Configurate(GetElements(Host));

            LinkDirectory d = new LinkDirectory();
            ResourceLinker = d;
            d.Configurate(GetElements(d));

            RedirectConfigure = new RedirectConfig();
			RedirectConfigure.Configurate(GetElements(RedirectConfigure));

			Collector = new SessionCollect();
			Collector.Configurate(GetElements(Collector));

			if (DirReader != null) {
				DirReader.Configurate(GetElements(DirReader));
			}

			Users = new UserBank();
			Users.Configurate(GetElements(Users));
		}
	}
}
