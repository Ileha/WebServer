using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Host.HttpHandler;
using Host.ServerExceptions;
using System.Net.Sockets;
using System.IO;
using Host.ConnectionHandlers.ExecutorExceptions;

namespace Host.ConnectionHandlers
{
    public class Reqest : IDisposable
    {
        private static byte[] new_line = new byte[] { 13, 10, 13, 10 };

        public string URL;
        private MemoryStream Data;
        public Dictionary<string, string> varibles;//данные
        public Dictionary<string, string> preferens;//заголовки
        public Dictionary<string, string> cookies;

        public static Reqest Create(TcpClient client, out Func<Stream> Input) {
            Reqest res = new Reqest(client);
            Input = () => res.Data;
            return res;
        }

        public Reqest(TcpClient client)
        {
            varibles = new Dictionary<string, string>();
            preferens = new Dictionary<string, string>();
            cookies = new Dictionary<string, string>();

            StringBuilder res = new StringBuilder();
            byte[] bytes = new byte[1024];
            NetworkStream stream = client.GetStream();
            int index = -1;
            Reqest result = this;
            string[] elements;
            string headers_data = "";

			do {
				int count = client.Client.Receive(bytes);
                ExistSeqeunce(0, count, new_line, bytes, out index);
				if (index == -1)
				{
					res.Append(Encoding.UTF8.GetString(bytes, 0, count));
				}
				else {
					res.Append(Encoding.UTF8.GetString(bytes, 0, index));
					headers_data = res.ToString();
                    elements = Regex.Split(headers_data, "\r\n");
                    try {
                        string[] header = elements[0].Split(' ');
						ABSHttpHandler _handler = Repository.ReqestsHandlers[header[0] + header[2]];
						_handler.ParseHeaders(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
						if (_handler.CanHasData(this)) {
							try
							{
								int data_lenght = Convert.ToInt32(preferens["Content-Length"]);
								Data = new MemoryStream(data_lenght);
								Data.Write(bytes, 0, count - (index + 4));
								bytes = new byte[data_lenght-(count - (index + 4))];
								count = client.Client.Receive(bytes);
								Data.Write(bytes, 0, count);
								Data.Seek(0, SeekOrigin.Begin);
							}
							catch (Exception err) {
								Data = new MemoryStream();
							}
						}
						else {
							Data = new MemoryStream();
						}
					}
                    catch (ExceptionCode code) {
                        throw code;
                    }
                    catch (Exception err) {
                        throw Repository.ExceptionFabrics["Bad Request"].Create(null, null);
                    }
				}
			} while (index == -1);

            
        }

        public void CheckTabelOfRedirect()
        {
            string new_url;
            try
            {
                new_url = Repository.Configurate.RedirectConfigure.GetTargetRedirect(URL);
            }
            catch (Exception err)
            {
                return;
            }
            throw Repository.ExceptionFabrics["Moved Permanently"].Create(null, new_url);
        }

        private static bool ExistSeqeunce(int start, int count, byte[] sequence, IEnumerable<byte> array, out int index)
        {
            int seq_i = 0;
            for (int i = start; i < Math.Min(count, array.Count()-start); i++)
            {
                if (array.ElementAt(i) != sequence[seq_i])
                {
                    i = i - seq_i;
                    seq_i = 0;
                }
                else
                {
                    if (seq_i == sequence.Length - 1)
                    {
                        index = i - seq_i;
                        return true;
                    }
                    seq_i++;
                }
            }
            index = -1;
            return false;
        }


        public void Dispose()
        {
            if (Data != null) {
                Data.Dispose();
            }
        }
    }
}
