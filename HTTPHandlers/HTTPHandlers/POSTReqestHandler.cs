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

		public override string HandlerType { get { return "POST"; } }

		public override string HandlerVersion { get { return "HTTP/1.1"; } }

		public override void ParseData(ref Reqest output, TcpClient data_sourse)
		{
			int lenght;
			try {
				lenght = Convert.ToInt32(output.preferens["Content-Length"]);
			}
			catch (KeyNotFoundException err) {
				throw new BadRequest();
			}
			catch (Exception err) {
				throw new InternalServerError();
			}
			byte[] buffer = new byte[lenght];
			data_sourse.Client.ReceiveTimeout = 10000;
			data_sourse.Client.Receive(buffer);
			output.varibles.Add("data", Encoding.UTF8.GetString(buffer, 0, lenght));
			Console.WriteLine("data : {0}", output.varibles["data"]);
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
                if (head == "Cookie") {
                    MatchCollection elements = for_cookie.Matches(m_pref.Groups["val"].Value);
                    foreach (Match element_of_elements in elements) {
                        output.cookies.Add(element_of_elements.Groups["name"].Value, element_of_elements.Groups["val"].Value);
						//Console.WriteLine("{0} : {1}", element_of_elements.Groups["name"].Value, element_of_elements.Groups["val"].Value);
                    }
                }
                else if (head != "") {
                    output.preferens.Add(m_pref.Groups["name"].Value, m_pref.Groups["val"].Value);
                }
            }
		}
	}
}
