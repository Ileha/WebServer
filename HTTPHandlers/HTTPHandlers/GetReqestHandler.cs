using System.Text.RegularExpressions;
using Host.ServerExceptions;
using Host;
using Host.HttpHandler;
using System;
using Host.ConnectionHandlers;

namespace HttpHandlers
{
	public class GetReqestHandler : ABSHttpHandler
	{
		Regex url_var = new Regex("^(?<url>[^=&]+)(\\?(?<var>[\\w=&]+))?");
		Regex name_val = new Regex("(?<name>[\\w]+)=(?<val>[\\w]+)");
		Regex pref_val = new Regex("(?<name>[\\w-]+):[ ]?(?<val>.+)");
		Regex TwoPoints = new Regex("\\.{2}");
		Regex for_cookie = new Regex("(?<name>[^\\s=;]+)=(?<val>[^=;]+)");

		public override string HandlerType { get { return "GET"; } }
		public override string HandlerVersion { get { return "HTTP/1.1"; } }

		public override void ParseHeaders(ref Reqest output, string[] reqest, string URI) {
			Match m = url_var.Match(URI);
			output.URL = m.Groups["url"].Value;
			if (TwoPoints.IsMatch(output.URL)) {//проверить на наличие двух точек подряд
                throw Repository.ExceptionFabrics["Bad Request"].Create(null, null);
			}
			foreach (Match s in name_val.Matches(m.Groups["var"].Value)) {//парсинг данных
				output.varibles.Add(s.Groups["name"].Value, s.Groups["val"].Value);
			}
			foreach (string s in reqest) {//парсинг заголовков
				Match m_pref = pref_val.Match(s);
				string head = m_pref.Groups["name"].Value;
				if (head == "Cookie") {
					MatchCollection elements = for_cookie.Matches(m_pref.Groups["val"].Value);
					foreach (Match element_of_elements in elements) {
						output.cookies.Add(element_of_elements.Groups["name"].Value, element_of_elements.Groups["val"].Value);
					}
				}
				else if (head != "") {
					output.preferens.Add(head, m_pref.Groups["val"].Value);
				}
			}
		}
		//public override void ParseData(ref Reqest output, string data_sourse) {
		//	throw new NotImplementedException();
		//}

		public override bool CanHasData(Reqest output) {
			return false;
		}

		public override long GetDataLenght(Reqest output)
		{
			throw new NotImplementedException();
		}
	}
}