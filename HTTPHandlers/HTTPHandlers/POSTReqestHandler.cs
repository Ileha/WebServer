using System;
using Host.ServerExceptions;
using Host;
using Host.HttpHandler;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using Host.HeaderData;
using Host.DataInput;

namespace HttpHandlers
{
	public class POSTReqestHandler : IHttpHandler
	{
		Regex pref_val = new Regex("(?<name>[\\w-]+):[ ]?(?<val>.+)");
		Regex TwoPoints = new Regex("\\.{2}");
		Regex for_cookie = new Regex("(?<name>[^\\s=;]+)=(?<val>[^=;]+)");
		Regex data_type01 = new Regex("(?<name>[^\\s=&]+)=(?<val>[^=&]+)");

		public override string HandlerType { get { return "POST"; } }

		public override string HandlerVersion { get { return "HTTP/1.1"; } }

		public override void ParseData(ref Reqest output, string data_sourse)
		{
			Console.WriteLine(data_sourse);
			//Regex.Replace(data_sourse,  ;
			string data_type = output.preferens["Content-Type"].Value["0"];
			if (data_type == "application/x-www-form-urlencoded") {
				//login=Petya%20Vasechkin&password=qq
				MatchCollection collect = data_type01.Matches(data_sourse);
				foreach (Match m in collect) {
					StringData dat = new StringData(m.Groups["name"].Value, m.Groups["val"].Value);
					output.varibles.Add(dat.Name, dat);
				}
			}
			else if (data_type == "multipart/form-data") {
				string for_split = "--" + output.preferens["Content-Type"].Value["boundary"];
				string[] data_parts = Regex.Split(data_sourse, for_split);
				foreach (string s in data_parts) {
					if (s == "") { continue; }
					string[] vals = Regex.Split(s, "\r\n\r\n");//первая заголовки; вторая данные
					string[] headers = Regex.Split(vals[0], "\r\n");//заголвки
					ABSReqestData data_to_return;
					Dictionary<string, HeaderValueMain> val = new Dictionary<string, HeaderValueMain>();
					for (int i = 0; i < headers.Length; i++) {
						if (headers[i] != "") {
							HeaderValueMain val_to_add = new HeaderValueMain(headers[i]);
							val.Add(val_to_add.Name, val_to_add);
						}
					}
					if (val.ContainsKey("Content-Type")) {
						data_to_return = new FileData(val["Content-Disposition"].Value["name"], Encoding.UTF8.GetBytes(vals[1]), val);
					}
					else {
						data_to_return = new StringData(val["Content-Disposition"].Value["name"], vals[1], val);
					}
					output.varibles.Add(data_to_return.Name, data_to_return);
				}
			}
			else {
				throw new BadRequest();
			}
			Console.WriteLine();
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
					}
				}
				else if (head != "") {
					HeaderValueMain new_header = new HeaderValueMain(s);
					output.preferens.Add(new_header.Name, new_header);
                }
            }
		}

		public override bool CanHasData(Reqest output) {
			try {
				if (Convert.ToInt32(output.preferens["Content-Length"].Value["0"]) != 0) {
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

		public override int GetDataLenght(Reqest output) {
			return Convert.ToInt32(output.preferens["Content-Length"].Value["0"]);
		}
	}
}
