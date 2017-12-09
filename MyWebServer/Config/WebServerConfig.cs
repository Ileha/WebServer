using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace Config
{
    public class ResoursePull : ReactorPull
    {
        private string _def;
        private Regex path;
        public ResoursePull(string def_path) : base() {
            _def = def_path;
            path = new Regex(@"^/");
        }
        public override string GetDefaultValue() {
            return _def;
        }
        public override ReactionValue Adder(XElement item)
        {
            if (Directory.Exists(item.Value)) {
                DirectoryInfo inf = new DirectoryInfo(item.Value);
                return new ReactionValue("^"+inf.Name, inf.Parent.FullName);
            }
            else if (File.Exists(item.Value)) {
                FileInfo inf = new FileInfo(item.Value);
                //string on_add = "";
                //try {
                //    on_add = item.Attribute("virtual_path").Value;
                //    on_add = path.Replace(on_add, "");
                //}
                //catch (Exception err) { }
                //Console.WriteLine(Path.Combine(on_add, inf.Name));
                return new ReactionValue("^" + inf.Name, inf.DirectoryName);
            }
            else {
                throw new FileNotFoundException("not found", item.Value);
            }
        }
        public override bool Mather(string get_resourse, ReactionValue out_resourse)
        {
            return out_resourse.Reactor.IsMatch(get_resourse);
        }
    }

	public class RedirectConfig : ReactorPull
	{
        public RedirectConfig() : base() { }
        public override ReactionValue Adder(XElement item)
        {
            return new ReactionValue("^" + item.Attribute("path").Value + "$", item.Attribute("target").Value);
        }
        public override string GetDefaultValue() { throw new NotImplementedException(); }
        public override bool Mather(string get_resourse, ReactionValue out_resourse)
        {
            return out_resourse.Reactor.IsMatch(get_resourse) && get_resourse != out_resourse.ReturnValue;
        }
	}

    public class WebServerConfig : IConfigRead {
        private Dictionary<string, XElement> _body_conf;
        public ResoursePull _resourses;
        public readonly RedirectConfig RedirectConfigure;

        public WebServerConfig(XElement body) {
            _body_conf = new Dictionary<string, XElement>();
            RedirectConfigure = new RedirectConfig();
            _resourses = new ResoursePull(body.Element("root_dir").Value);
            foreach (XElement el in body.Elements()) {
                if (el.Name.LocalName == "redirect_table") {
                    RedirectConfigure.Configure(el);
                }
                if (el.Name.LocalName == "additive_dirs") {
                    _resourses.Configure(el);
                }
                else if (!el.HasElements) {
                    _body_conf.Add(el.Name.LocalName, el);
                }
            }
        }

        public void Remove(string index) {
            _body_conf.Remove(index);
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
