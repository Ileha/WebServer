using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Config
{
    class RedirectConfig : MarshalByRefObject, IConfigRead {
        private Dictionary<string, string> _body_redirect;

        public RedirectConfig() {
            _body_redirect = new Dictionary<string, string>();
        }

        public void Configure(XElement body) {
            _body_redirect = new Dictionary<string, string>();
            foreach (XElement el in body.Elements()) {
                _body_redirect.Add(el.Attribute("path").Value, el.Attribute("target").Value);
            }
        }

        public string this[string index] {
            get {
                try {
                    return _body_redirect[index];
                }
                catch (Exception err) {
                    throw new ErrorServerConfig(index);
                }
            }
        }
    }

    public class WebServerConfig : MarshalByRefObject, IConfigRead {
        private Dictionary<string, string> _body_conf;
        public readonly IConfigRead RedirectConfigure;

        public WebServerConfig(XElement body) {
            _body_conf = new Dictionary<string, string>();
            RedirectConfig r = new RedirectConfig();
            RedirectConfigure = r;
            foreach (XElement el in body.Elements()) {
                if (el.Name.LocalName == "redirect_table") {
                    r.Configure(el);
                }
                else if (!el.HasElements) {
                    _body_conf.Add(el.Name.LocalName, el.Value);
                }
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
