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
using Host.HeaderData;

namespace Host {
    public class Reqest {
		private static Regex doubal_new_string = new Regex("\r\n\r\n");

        public string URL;
        public Dictionary<string, ABSReqestData> varibles;
        public Dictionary<string, HeaderValueMain> preferens;
		public Dictionary<string, string> cookies;

        public Reqest() {
            varibles = new Dictionary<string, ABSReqestData>();
            preferens = new Dictionary<string, HeaderValueMain>();
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

		private static string Receive(TcpClient client, long length) {
			long max = length;
			byte[] buffer = new byte[max];
			string request = "";
			int count;
			while (max > 0) {
				count = client.GetStream().Read(buffer, 0, buffer.Length);
				request += Encoding.UTF8.GetString(buffer, 0, count);
				max -= count;
			}
			Console.WriteLine(request);
			return request;
		}

        public static Reqest CreateNewReqest(string reqest, TcpClient client) {
            Reqest result = new Reqest();
			string[] headers_data = new string[2];
			Match m = doubal_new_string.Match(reqest);
			headers_data[0] = reqest.Substring(0, m.Index);
			if (m.Index + 4 < reqest.Length) {
				headers_data[1] = reqest.Substring(m.Index + 4);
			}
            string[] elements = Regex.Split(headers_data[0], "\r\n");
            try {
                string[] header = elements[0].Split(' ');
                IHttpHandler _handler = Repository.ReqestsHandlers[header[0]+header[2]];
                _handler.ParseHeaders(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
				if (_handler.CanHasData(result)) {
					if (headers_data[1] != null) {
						long _length = _handler.GetDataLenght(result) - headers_data[1].Length;
						if (_length > 0) {
							_handler.ParseData(ref result, headers_data[1]+Receive(client, _length));
						}
						else {
							_handler.ParseData(ref result, headers_data[1]);
						}
					}
					else {
						_handler.ParseData(ref result, Receive(client, _handler.GetDataLenght(result)));
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
