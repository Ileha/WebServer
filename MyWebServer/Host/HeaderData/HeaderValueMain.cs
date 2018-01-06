using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Host.HeaderData
{
	public class HeaderValue {
		private static Regex eq = new Regex("=");
		private static Regex simple = new Regex("(?<name>[^=]+)=(?<val>[^=]+)");

		public Dictionary<string, string> Value {
			get {
				return _value;
			}
		}
		private Dictionary<string, string> _value;

		public HeaderValue(string pars_string) {
			_value = new Dictionary<string, string>();
			string[] vals = Regex.Split(pars_string, ";");
			int i = 0;
			foreach (string str in vals) {
				if (str != "") {
					if (eq.IsMatch(str)) {
						Match match = simple.Match(str);
						Value.Add(match.Groups["name"].Value.Trim(), match.Groups["val"].Value);
					}
					else {
						Value.Add(i.ToString(), str);
					}
					i++;
				}
			}
		}
	}

	public class HeaderValueMain {
		private static Regex first = new Regex("(?<name>[^:]+):[ ]?(?<val>[^,]+)");
		private static Regex eq = new Regex("=");
		private static Regex simple = new Regex("(?<name>[^=]+)=(?<val>[^=]+)");

		public string Name;
		public Dictionary<string, string> Value { 
			get {
				return _value;
			} 
		}
		private Dictionary<string, string> _value;

		private HeaderValue[] bank;

		public HeaderValueMain(string pars_string) {
			_value = new Dictionary<string, string>();
			Match m = first.Match(pars_string);
			string after = pars_string.Substring(m.Length);
			Name = m.Groups["name"].Value;
			string[] vals = Regex.Split(m.Groups["val"].Value, ";");
			int i = 0;
			foreach (string str in vals) {
				if (str != "") {
					if (eq.IsMatch(str)) {
						Match match = simple.Match(str);
						Value.Add(match.Groups["name"].Value.Trim(), match.Groups["val"].Value);
					}
					else {
						Value.Add(i.ToString(), str);
					}
					i++;
				}
			}
			if (after.Length == 0) { return; }
			vals = Regex.Split(after, ",");
			i = 0;
			foreach (string str in vals) {
				if (str != "") { i++; }
			}
			bank = new HeaderValue[i];
			foreach (string str in vals) {
				if (str != "") {
					bank[bank.Length - i] = new HeaderValue(str);
					i--;
				}
			}
		}

		public HeaderValue[] GetAny() {
			return bank;
		}
	}
}
