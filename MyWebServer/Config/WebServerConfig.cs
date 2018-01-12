using System;
using System.Xml.Linq;
using Resouces;
using Host;

namespace Config
{
	public class WebServerConfig {
        private XElement _body_conf;
		public XElement ConfigBody { 
			get {
				return _body_conf;	
			} 
		}
        public readonly RedirectConfig RedirectConfigure;
        public IItem ResourceLinker;
		public WebSerwer Host;

        public WebServerConfig(XElement body) {
			_body_conf = body;

            LinkDirectory d = new LinkDirectory();
			ResourceLinker = d;
			d.Configurate(body.Element(d.ConfigName));

            RedirectConfigure = new RedirectConfig();
			RedirectConfigure.Configurate(body.Element(RedirectConfigure.ConfigName));

			Host = new WebSerwer();
			Host.Configurate(body.Element(Host.ConfigName));
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
    }
}
