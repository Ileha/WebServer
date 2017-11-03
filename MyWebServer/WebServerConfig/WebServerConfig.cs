using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MyWebServer.WebServerConfigure
{
    public class WebServerConfig : IConfigRead {
        private Dictionary<string, string> _body_conf;

        public WebServerConfig(XElement body) {
            _body_conf = new Dictionary<string, string>();
            foreach (XElement el in body.Elements()) {
                _body_conf.Add(el.Name.LocalName, el.Value);
            }
        }

        public void Remove(string index) {
            _body_conf.Remove(index);
        }

        public string this[string index] {
            get {
                try {
                    return _body_conf[index];
                }
                catch (Exception err) {
                    throw new ErrorServerConfig(index);
                }
            }
            set {
                try {
                    _body_conf[index] = value;
                }
                catch (Exception err) {
                    throw new ErrorServerConfig(index);
                }
            }
        }
    }
}
