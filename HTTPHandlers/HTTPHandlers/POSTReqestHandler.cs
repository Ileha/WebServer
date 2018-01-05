using System;
using Host.ServerExceptions;
using Host;
using Host.HttpHandler;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace HttpHandlers
{
	public class POSTReqestHandler : IHttpHandler
	{
		Regex pref_val = new Regex("(?<name>[\\w-]+):[ ]?(?<val>.+)");
		Regex TwoPoints = new Regex("\\.{2}");
		Regex for_cookie = new Regex("(?<name>[^\\s=;]+)=(?<val>[^=;]+)");
		Regex data = new Regex("^(?<val>[^;]+)");

		public override string HandlerType { get { return "POST"; } }

		public override string HandlerVersion { get { return "HTTP/1.1"; } }

		public override void ParseData(ref Reqest output, string data_sourse)
		{
			//int lenght;
			//try {
			//	lenght = Convert.ToInt32(output.preferens["Content-Length"]);
			//}
			//catch (KeyNotFoundException err) {
			//	throw new BadRequest();
			//}
			//catch (Exception err) {
			//	throw new InternalServerError();
			//}
			//byte[] buffer = new byte[lenght];
			//data_sourse.Client.ReceiveTimeout = 10000;
			//data_sourse.Client.Receive(buffer);
			//string data_str = Encoding.UTF8.GetString(buffer, 0, lenght);
			//string[] data_array = Regex.Split(data_str, "--"+output.preferens["Content-Type.boundary"]);
			Console.WriteLine(data_sourse);
			//output.varibles.Add("data", Encoding.UTF8.GetString(buffer, 0, lenght));
			//Console.WriteLine("data : {0}", output.varibles["data"]);
		}

		public override void ParseHeaders(ref Reqest output, string[] reqest, string URI)
		{
			output.URL = URI;
            if (TwoPoints.IsMatch(output.URL)) {//проверить на наличие двух точек подряд
                throw new BadRequest();
            }
            foreach (string s in reqest) {
                Match m_pref = pref_val.Match(s);
				string head = m_pref.Groups["name"].Value;
				if (head == "Cookie")
				{
					MatchCollection elements = for_cookie.Matches(m_pref.Groups["val"].Value);
					foreach (Match element_of_elements in elements)
					{
						output.cookies.Add(element_of_elements.Groups["name"].Value, element_of_elements.Groups["val"].Value);
						//Console.WriteLine("{0} : {1}", element_of_elements.Groups["name"].Value, element_of_elements.Groups["val"].Value);
					}
				}
				else if (head == "Content-Type") {
					string datas = m_pref.Groups["val"].Value;
					output.preferens.Add(head, data.Match(datas).Groups["val"].Value);
					MatchCollection pref = for_cookie.Matches(datas);
					foreach (Match m in pref) {
						output.preferens.Add(head+"."+m.Groups["name"].Value, m.Groups["val"].Value);
					}

				}
				else if (head != "") {
					//string 
                    output.preferens.Add(head, m_pref.Groups["val"].Value);
                }
            }
		}

		public override bool CanHasData(Reqest output)
		{
			try {
				if (Convert.ToInt32(output.preferens["Content-Length"]) != 0) {
					return true;
				}
				else {
					return false;
				}
			}
			catch (Exception err) {
				return false;
			}
		}
	}
}
