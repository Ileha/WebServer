using System;
using System.Xml.Linq;

namespace Config
{
	public class RedirectConfig : ReactorPull
	{
        private string[] names = new string[] { "redirect_table" };
		public RedirectConfig() : base() { }
		public override string[] ConfigName {
			get {
				return names;
			}
		}
		public override ReactionValue Adder(XElement item) {
			return new ReactionValue("^" + item.Attribute("path").Value + "$", item.Attribute("target").Value);
		}
		public override string GetDefaultValue(string url) { throw new NotImplementedException(); }
		public override bool Сomparer(string get_resourse, ReactionValue out_resourse)
		{
			return out_resourse.Reactor.IsMatch(get_resourse) && get_resourse != out_resourse.ReturnValue;
		}	
	}
}
