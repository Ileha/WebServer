using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Text;
using RequestHandlers;
using Configurate.Host.Connection.HTTPConnection;

namespace HttpHandlers
{
    public class GetReqestHandler : ABSHttpHandler
    {
        private static byte[] new_line = new byte[] { 13, 10, 13, 10 };
        Regex url_var = new Regex("^(?<url>[^=&]+)(\\?(?<var>[\\w=&]+))?");
        //Regex name_val = new Regex("(?<name>[\\w]+)=(?<val>[\\w]+)");
        Regex pref_val = new Regex("(?<name>[\\w-]+):[ ]?(?<val>.+)");
        Regex TwoPoints = new Regex("\\.{2}");
        Regex for_cookie = new Regex("(?<name>[^\\s=;]+)=(?<val>[^=;]+)");

        public override string HandlerType { get { return "GET"; } }
        public override string HandlerVersion { get { return "HTTP/1.1"; } }

        public override void ParseHeaders(ref Reqest output, Stream reqest)
        {
            StringBuilder res = new StringBuilder();
            byte[] bytes = new byte[1024];
            int _index = -1;
            do 
            {
                int _count = reqest.Read(bytes, 0, bytes.Length);
                Reqest.ExistSeqeunce(0, _count, new_line, bytes, out _index);
                res.Append(Encoding.UTF8.GetString(bytes, 0, Math.Max(_index, _count)));
            } while (_index == -1);
            string[] headers = Regex.Split(res.ToString(), "\r\n");

            Match m = url_var.Match(headers[0].Split(' ')[1]);
            output.URL = m.Groups["url"].Value;
            if (TwoPoints.IsMatch(output.URL)) {
                //проверить на наличие двух точек подряд
                throw Repository.ExceptionFabrics["Bad Request"].Create(null, null);
            }
            byte[] data = Encoding.UTF8.GetBytes(m.Groups["var"].Value);
            output.Data.Write(data, 0, data.Length);
            output.Data.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i < headers.Length; i++) {
                //парсинг заголовков
                Match m_pref = pref_val.Match(headers[i]);
                string head = m_pref.Groups["name"].Value;
                if (head == "Cookie")
                {
                    MatchCollection elements = for_cookie.Matches(m_pref.Groups["val"].Value);
                    foreach (Match element_of_elements in elements)
                    {
                        output.cookies.Add(element_of_elements.Groups["name"].Value, element_of_elements.Groups["val"].Value);
                    }
                }
                else if (head != "")
                {
                    output.headers.Add(head, m_pref.Groups["val"].Value);
                }
            }
        }
    }
}