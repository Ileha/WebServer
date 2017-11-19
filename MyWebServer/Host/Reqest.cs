using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Host.HttpHandler;
using Host.ServerExceptions;
using Config;

namespace Host {

    public class Reqest {
        public string URL;
        public Dictionary<string, string> varibles;
        public Dictionary<string, string> preferens;

        public Reqest() {
            varibles = new Dictionary<string, string>();
            preferens = new Dictionary<string, string>();
        }

        public static Reqest CreateNewReqest(string reqest) {
            Reqest result = new Reqest();
            string[] elements = Regex.Split(reqest, "\r\n");
            try {
                string[] header = elements[0].Split(' ');
                IHttpHandler _handler = Repository.ReqestsHandlers[header[0]];
                _handler.Parse(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
            }
            catch (Exception err) {
                if (err is ExceptionCode) {
                    throw err;
                }
                else {
					throw new BadRequest();
                }
            } 
            return result;
        }
    }
}
