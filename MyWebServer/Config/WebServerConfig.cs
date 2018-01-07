using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Resouces;

namespace Config
{
	public class RedirectConfig : ReactorPull
	{
        public RedirectConfig() : base() { }
        public override ReactionValue Adder(XElement item)
        {
            return new ReactionValue("^" + item.Attribute("path").Value + "$", item.Attribute("target").Value);
        }
        public override string GetDefaultValue(string url) { throw new NotImplementedException(); }
        public override bool Сomparer(string get_resourse, ReactionValue out_resourse)
        {
            return out_resourse.Reactor.IsMatch(get_resourse) && get_resourse != out_resourse.ReturnValue;
        }
	}

    public class WebServerConfig : IConfigRead {
        private Dictionary<string, XElement> _body_conf;
        public readonly RedirectConfig RedirectConfigure;
        public IItem ResourceLinker;

        public WebServerConfig(XElement body) {
            _body_conf = new Dictionary<string, XElement>();
            ResourceLinker = new Resouces.LinkDirectory(new DirectoryInfo(body.Element("root_dir").Value), null);
            RedirectConfigure = new RedirectConfig();
            foreach (XElement el in body.Elements()) {
                if (el.Name.LocalName == "redirect_table") {
                    RedirectConfigure.Configure(el);
                }
                if (el.Name.LocalName == "additive_dirs") {
                    foreach (XElement add_dir in el.Elements())
                    {
                        if (Directory.Exists(add_dir.Value))
                        {
                            ResourceLinker.AddItem(new LinkDirectory(new DirectoryInfo(add_dir.Value), ResourceLinker));
                        }
                        else if (File.Exists(add_dir.Value))
                        {
                            ResourceLinker.AddItem(new LinkFile(new FileInfo(add_dir.Value), ResourceLinker));
                        }
                        else {}
                    }
                }
                else if (!el.HasElements) {
                    _body_conf.Add(el.Name.LocalName, el);
                }
            }
        }

        public void Remove(string index) {
            _body_conf.Remove(index);
        }
		public void AddElement(XElement element) {
			_body_conf.Add(element.Name.LocalName, element);
		}

		public XElement GetElement(string name)
		{
			try
			{
				return new XElement(_body_conf[name]);
			}
			catch (Exception err) {
				throw new ErrorServerConfig(name);
            }
		}

		public XElement this[string index] {
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
