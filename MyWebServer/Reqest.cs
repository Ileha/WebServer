using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MyWebServer.HttpHandler;
using MyWebServer.ServerExceptions;

namespace MyWebServer
{
    public enum TypeReqest {
        GET,
        POST
    }

    public class Reqest {
        public string URL;
        public Dictionary<string, string> varibles;
        public Dictionary<string, string> preferens;

        public Reqest() {
            varibles = new Dictionary<string, string>();
            preferens = new Dictionary<string, string>();
        }

        public static Reqest CreateNewReqest(string reqest, Func<TypeReqest, IHttpHandler> getHTTPHandl) {
            Reqest result = new Reqest();
            string[] elements = Regex.Split(reqest, "\r\n");
            try {
                string[] header = elements[0].Split(' ');
                IHttpHandler _handler = getHTTPHandl((TypeReqest)Enum.Parse(typeof(TypeReqest), header[0]));
                _handler.Parse(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
            }
            catch (Exception err) {
                if (err is ExceptionCode) {
                    throw err;
                }
                else {
                    throw ExceptionCode.BadRequest();
                }
            } 
            return result;
        }
    }
}
