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
    public abstract class ReactorPull
    {
        private List<ReactionValue> _list_of_redirect;

        public ReactorPull()
		{
			_list_of_redirect = new List<ReactionValue>();
		}

        abstract public bool Mather(string get_resourse, ReactionValue out_resourse);
        abstract public string GetDefaultValue();

		public string GetTargetRedirect(string url)
		{
			foreach (ReactionValue RV in _list_of_redirect)
			{
				if (Mather(url,RV))
				{
					return RV.ReturnValue;
				}
			}
            try {
                return GetDefaultValue();
            }
            catch (Exception err) { throw new RedirectNotFound(url); }
		}

        abstract public ReactionValue Adder(XElement item);

		public void Configure(XElement body)
		{
			foreach (XElement el in body.Elements())
			{
                try
                {
                    _list_of_redirect.Add(Adder(el));
                }
                catch (FileNotFoundException err) { continue; }
			}
		}
    }
}
