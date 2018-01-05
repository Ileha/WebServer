using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Host.HttpHandler;
using Host.ServerExceptions;
using Config;
using System.Reflection;
using System.Net.Sockets;
using Host.DataInput;

namespace Host {

    public class Reqest {
        public string URL;
        public Dictionary<string, ABSReqestData> varibles;
        public Dictionary<string, string> preferens;
		public Dictionary<string, string> cookies;

        public Reqest() {
            varibles = new Dictionary<string, ABSReqestData>();
            preferens = new Dictionary<string, string>();
			cookies = new Dictionary<string, string>();
        }

        public void Redirect(string targeURL) {
            throw new MovedPermanently(targeURL);
        }

        public void CheckTabelOfRedirect() {
            string new_url;
            try {
                new_url = Repository.Configurate.RedirectConfigure.GetTargetRedirect(URL);
            }
            catch (Exception err) { 
                return;
            }
            throw new MovedPermanently(new_url);
        }

		private static string Receive(TcpClient client) {
			byte[] buffer = new byte[1024];
			string request = "";
			int count;

            while ((count = client.GetStream().Read(buffer, 0, buffer.Length)) > 0) {
                request += Encoding.UTF8.GetString(buffer, 0, count);
                if (request.IndexOf("\r\n\r\n") >= 0) { //Запрос обрывается \r\n\r\n последовательностью
                    break;
                }
            }
			return request;
		}

        public static Reqest CreateNewReqest(string reqest, TcpClient client) {
            Reqest result = new Reqest();
			string[] headers_data = Regex.Split(reqest, "\r\n\r\n");
            string[] elements = Regex.Split(headers_data[0], "\r\n");
            try {
                string[] header = elements[0].Split(' ');
                IHttpHandler _handler = Repository.ReqestsHandlers[header[0]+header[2]];
                _handler.ParseHeaders(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
				if (_handler.CanHasData(result)) {
					try {
						if (headers_data[1] != "") {
							_handler.ParseData(ref result, headers_data[1]);
						}
						else {
							_handler.ParseData(ref result, Receive(client));
						}
					}
					catch(IndexOutOfRangeException err) {
						_handler.ParseData(ref result, Receive(client));
					}
				}
            }
            catch (Exception err) {
                if (err.GetType().IsSubclassOf(typeof(ExceptionCode))) {
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
