using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Configurate.Redirect.Exception;

namespace Configurate.Redirect
{
    class ReactionValue
    {
        public readonly Regex Reactor;
        public readonly string ReturnValue;

        public ReactionValue(string reaction, string target_value)
        {
            Reactor = new Regex(reaction);
            ReturnValue = target_value;
        }
    }
    public class RedirectConfig : IConfigurate
    {
        private List<ReactionValue> _list_of_redirect;

		private string[] names = new string[] { "redirect_table" };
		public string[] ConfigName {
			get {
				return names;
			}
		}

        public RedirectConfig() {
			_list_of_redirect = new List<ReactionValue>();
		}

		public string GetTargetRedirect(string url) {
			foreach (ReactionValue RV in _list_of_redirect) {
                if (RV.Reactor.IsMatch(url) && url != RV.ReturnValue) {
                    return RV.ReturnValue;
				}
			}
            throw new RedirectNotFound(url);
		}

		public virtual void Configurate(XElement data) {
			foreach (XElement el in data.Elements()) {
                try {
                    _list_of_redirect.Add(new ReactionValue("^" + el.Attribute("path").Value + "$", el.Attribute("target").Value));
                }
                catch (FileNotFoundException err) { continue; }
			}
		}
    }
}
