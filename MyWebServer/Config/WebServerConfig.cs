using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Config
{
    class ReactionValue {
        public readonly Regex Reactor;
        public readonly string ReturnValue;

        public ReactionValue(string reaction, string target_value) {
            Reactor = new Regex(reaction);
            ReturnValue = target_value;
        }
    }
	public class RedirectConfig
	{
		//private Dictionary<Regex, string> _body_redirect;
		private List<ReactionValue> _list_of_redirect;

		public RedirectConfig()
		{
			//_body_redirect = new Dictionary<Regex, string>();
			_list_of_redirect = new List<ReactionValue>();
		}

		public string GetTargetRedirect(string url)
		{
			foreach (ReactionValue RV in _list_of_redirect)
			{
				if (RV.Reactor.IsMatch(url) && url != RV.ReturnValue)
				{
					return RV.ReturnValue;
				}
			}
			throw new RedirectNotFound(url);
		}

		public void AddRule(string reaction, string target_value)
		{
			_list_of_redirect.Add(new ReactionValue(reaction, target_value));
		}

		public void Configure(XElement body)
		{
			foreach (XElement el in body.Elements())
			{
				//_body_redirect.Add(new Regex(el.Attribute("path").Value), el.Attribute("target").Value);
				_list_of_redirect.Add(new ReactionValue("^" + el.Attribute("path").Value + "$", el.Attribute("target").Value));
			}
			Console.WriteLine();
		}
	}

    public class WebServerConfig : IConfigRead {
        private Dictionary<string, XElement> _body_conf;
        public readonly RedirectConfig RedirectConfigure;

        public WebServerConfig(XElement body) {
            _body_conf = new Dictionary<string, XElement>();
            RedirectConfigure = new RedirectConfig();
            foreach (XElement el in body.Elements()) {
                if (el.Name.LocalName == "redirect_table") {
                    RedirectConfigure.Configure(el);
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
