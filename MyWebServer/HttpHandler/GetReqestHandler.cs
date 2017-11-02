using System;
using System.Text.RegularExpressions;
using MyWebServer.ServerExceptions;

namespace MyWebServer.HttpHandler
{
    public class GetReqestHandler : IHttpHandler {
        Regex url_var = new Regex("^/(?<url>[\\w/\\.]+)(\\?(?<var>[\\w=&]+))?");
        Regex name_val = new Regex("(?<name>[\\w]+)=(?<val>[\\w]+)");
        Regex pref_val = new Regex("(?<name>[\\w-]+):[ ]?(?<val>.+)");
        Regex TwoPoints = new Regex("\\.{2}");


        public override TypeReqest HandlerType { get{ return TypeReqest.GET;}}
        public override string Version { get { return "HTTP/1.1"; } }

        public override void Parse(ref Reqest output, string[] reqest, string URI) {
            Match m = url_var.Match(URI);
            output.URL = m.Groups["url"].Value;
            if (TwoPoints.IsMatch(output.URL)) {//проверить на наличие двух точек подряд
                throw ExceptionCode.BadRequest();
            }
            foreach (Match s in name_val.Matches(m.Groups["var"].Value)) {
                output.varibles.Add(s.Groups["name"].Value, s.Groups["val"].Value);
            }
            foreach (string s in reqest) {
                Match m_pref = pref_val.Match(s);
                if (m_pref.Groups["name"].Value != "") {
                    output.preferens.Add(m_pref.Groups["name"].Value, m_pref.Groups["val"].Value);
                }
            }
        }
    }
}
