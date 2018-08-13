using System;
using Host;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RequestHandlers;
using Configurate.Host.Connection.HTTPConnection;

namespace HttpHandlers
{
	public class POSTReqestHandler : ABSHttpHandler
	{
        private static byte[] new_line = new byte[] { 13, 10, 13, 10 };
		Regex pref_val = new Regex("(?<name>[\\w-]+):[ ]?(?<val>.+)");
		Regex TwoPoints = new Regex("\\.{2}");
		Regex for_cookie = new Regex("(?<name>[^\\s=;]+)=(?<val>[^=;]+)");
		//Regex data_type01 = new Regex("(?<name>[^\\s=&]+)=(?<val>[^=&]+)");

		public override string HandlerType { get { return "POST"; } }

		public override string HandlerVersion { get { return "HTTP/1.1"; } }

		//public override void ParseData(ref Reqest output, string data_sourse)
		//{
		//	string data_type = output.preferens["Content-Type"].Value[0].Value["0"];
		//	if (data_type == "application/x-www-form-urlencoded") {
		//		MatchCollection collect = data_type01.Matches(data_sourse);
		//		foreach (Match m in collect) {
		//			StringData dat = new StringData(m.Groups["name"].Value, m.Groups["val"].Value);
		//			output.varibles.Add(dat.Name, dat);
		//		}
		//	}
		//	else if (data_type == "multipart/form-data") {
		//		string for_split = "--" + output.preferens["Content-Type"].Value[0].Value["boundary"];
		//		string[] data_parts = Regex.Split(data_sourse, for_split);
		//		foreach (string s in data_parts) {
		//			if (s == "" || s == "--\r\n") { continue; }
		//			string[] vals = Regex.Split(s, "\r\n\r\n");//первая заголовки; вторая данные
		//			string[] headers = Regex.Split(vals[0], "\r\n");//заголвки
		//			ABSReqestData data_to_return;
		//			Dictionary<string, HeaderValueMain> val = new Dictionary<string, HeaderValueMain>();
		//			for (int i = 0; i < headers.Length; i++) {
		//				if (headers[i] != "") {
		//					HeaderValueMain val_to_add = new HeaderValueMain(headers[i]);
		//					val.Add(val_to_add.Name, val_to_add);
		//				}
		//			}
		//			if (val.ContainsKey("Content-Type")) {
		//				data_to_return = new FileData(val["Content-Disposition"].Value[0].Value["name"], Encoding.UTF8.GetBytes(vals[1]), val);
		//			}
		//			else {
		//				data_to_return = new StringData(val["Content-Disposition"].Value[0].Value["name"], vals[1], val);
		//			}
		//			output.varibles.Add(data_to_return.Name, data_to_return);
		//		}
		//	}
		//	else {
		//		throw new BadRequest();
		//	}
		//}

        public override void ParseHeaders(ref Reqest output, Stream reqest)
		{
            StringBuilder res = new StringBuilder();
            byte[] bytes = new byte[1024];
            int _index = -1;
            int _count;
            do
            {
                _count = reqest.Read(bytes, 0, bytes.Length);
                Reqest.ExistSeqeunce(0, _count, new_line, bytes, out _index);
                if (_index == -1) {
                    res.Append(Encoding.UTF8.GetString(bytes, 0, _count));
                }
                else {
                    res.Append(Encoding.UTF8.GetString(bytes, 0, _index));
                }
                
            } while (_index == -1);
            string[] headers = Regex.Split(res.ToString(), "\r\n");
            output.Data.Write(bytes, _index + 4, _count - (_index + 4));

            output.URL = headers[0].Split(' ')[1];
            if (TwoPoints.IsMatch(output.URL)) {//проверить на наличие двух точек подряд
                throw Repository.ExceptionFabrics["Bad Request"].Create();
            }
            for (int i = 1; i < headers.Length; i++) {
                Match m_pref = pref_val.Match(headers[i]);
				string head = m_pref.Groups["name"].Value;
				if (head == "Cookie") {
					MatchCollection elements = for_cookie.Matches(m_pref.Groups["val"].Value);
					foreach (Match element_of_elements in elements) {
						output.cookies.Add(element_of_elements.Groups["name"].Value, element_of_elements.Groups["val"].Value);
					}
				}
				else if (head != "") {
					output.headers.Add(head, m_pref.Groups["val"].Value);
                }
            }
            try {
                bytes = new byte[Convert.ToInt32(output.headers["Content-Length"]) - (_count - (_index + 4))];
            }
            catch(Exception err) {
                throw Repository.ExceptionFabrics["Bad Request"].Create();
            }
            reqest.Read(bytes, 0, bytes.Length);
            output.Data.Write(bytes, 0, bytes.Length);
            output.Data.Seek(0, SeekOrigin.Begin);
		}
	}
}
