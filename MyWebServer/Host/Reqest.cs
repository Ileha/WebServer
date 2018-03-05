using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Host.HttpHandler;
using Host.ServerExceptions;
using System.Net.Sockets;
using Host.DataInput;
using Host.HeaderData;

namespace Host {
    public class Reqest {
        public string URL;
        public Dictionary<string, ABSReqestData> varibles;//данные после заголовков
        public Dictionary<string, HeaderValueMain> preferens;//заголовки
		public Dictionary<string, string> cookies;

        public Reqest() {
            varibles = new Dictionary<string, ABSReqestData>();
            preferens = new Dictionary<string, HeaderValueMain>();
			cookies = new Dictionary<string, string>();
        }

        public void CheckTabelOfRedirect() {
            string new_url;
            try {
                new_url = Repository.Configurate.RedirectConfigure.GetTargetRedirect(URL);
            }
            catch (Exception err) { 
                return;
            }
			throw Repository.ExceptionFabrics["Moved Permanently"].Create(new_url);
        }

		private static byte[] Receive(TcpClient client, long length) {
			long max = length;
			byte[] buffer = new byte[max];
			List<byte> request = new List<byte>();
			int count;
			while (max > 0) {
				count = client.GetStream().Read(buffer, 0, buffer.Length);
				request.AddRange(buffer);
				max -= count;
			}
			return request.ToArray();
		}

        public static Reqest CreateNewReqest(List<byte> reqest, int _index, TcpClient client) {
            Reqest result = new Reqest();
            string headers_data = Encoding.UTF8.GetString(reqest.GetRange(0, _index).ToArray());
            byte[] data = null;
			if (_index + 4 < reqest.Count) {
				data = reqest.GetRange(_index + 4, reqest.Count - (_index + 4)).ToArray();
			}
            string[] elements = Regex.Split(headers_data, "\r\n");
            try {
                string[] header = elements[0].Split(' ');
                IHttpHandler _handler = Repository.ReqestsHandlers[header[0]+header[2]];
                _handler.ParseHeaders(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
				if (_handler.CanHasData(result)) {
					if (data != null) {
						long _length = _handler.GetDataLenght(result) - data.Length;
						if (_length > 0) {
							_handler.ParseData(ref result, Encoding.UTF8.GetString(data.Concat(Receive(client, _length)).ToArray()));
						}
						else {
							_handler.ParseData(ref result, Encoding.UTF8.GetString(data));
						}
					}
					else {
						_handler.ParseData(ref result, Encoding.UTF8.GetString(Receive(client, _handler.GetDataLenght(result))));
					}
				}
            }
            catch (Exception err) {
                if (err.GetType().IsSubclassOf(typeof(ExceptionCode))) {
                    throw err;
                }
                else {
					throw Repository.ExceptionFabrics["Bad Request"].Create(null);
                }
            } 
            return result;
        }
    }
}
