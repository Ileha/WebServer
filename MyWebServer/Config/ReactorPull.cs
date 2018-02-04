using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Config
{
    public class ReactionValue
    {
        public readonly Regex Reactor;
        public readonly string ReturnValue;

        public ReactionValue(string reaction, string target_value)
        {
            Reactor = new Regex(reaction);
            ReturnValue = target_value;
        }
    }
    public abstract class ReactorPull : IConfigurate
    {
        protected List<ReactionValue> _list_of_redirect;

		public virtual string[] ConfigName { get { throw new NotImplementedException(); } }

		public ReactorPull() {
			_list_of_redirect = new List<ReactionValue>();
		}

        abstract public bool Сomparer(string get_resourse, ReactionValue out_resourse);
        abstract public string GetDefaultValue(string url);
		public virtual string OnCompaerReturn(ReactionValue RV, string get_path) {
			return RV.ReturnValue; 
		}
		public string GetTargetRedirect(string url)
		{
			foreach (ReactionValue RV in _list_of_redirect)
			{
				if (Сomparer(url,RV))
				{
					return OnCompaerReturn(RV, url);
				}
			}
            try {
                return GetDefaultValue(url);
            }
            catch (Exception err) { throw new RedirectNotFound(url); }
		}

        abstract public ReactionValue Adder(XElement item);

		public virtual void Configurate(XElement data) {
			foreach (XElement el in data.Elements()) {
                try {
                    _list_of_redirect.Add(Adder(el));
                }
                catch (FileNotFoundException err) { continue; }
			}
		}
    }
}
