using System;
using System.Xml.Linq;
using Resouces;
using Host;
using Host.DirReader;
using MainProgramm;
using Host.Session;

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
		public IDirectoryReader DirReader;
		public SessionCollect Collector;

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

		private void Configurate() {
            LinkDirectory d = new LinkDirectory();
			ResourceLinker = d;
			d.Configurate(_body_conf.Element(d.ConfigName));

            RedirectConfigure = new RedirectConfig();
			RedirectConfigure.Configurate(_body_conf.Element(RedirectConfigure.ConfigName));

			Host = new WebSerwer();
			Host.Configurate(_body_conf.Element(Host.ConfigName));

			Collector = new SessionCollect();
			Collector.Configurate(_body_conf.Element(Collector.ConfigName));

			if (DirReader != null) {
				DirReader.Configurate(_body_conf.Element(DirReader.ConfigName));
			}
		}
	}
}
