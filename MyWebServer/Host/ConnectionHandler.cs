using System;
using System.Net.Sockets;
using System.Text;
using Host.ServerExceptions;
using Config;
using Host.HttpHandler;
using Host.MIME;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Host {
    public class ConnectionHandler {
        public readonly TcpClient connection;

        public ExceptionCode code;
        public IHttpHandler handler;
        public Reader reads_bytes;
        public Reqest obj_request;
        public Response response;

        public ConnectionHandler(TcpClient Connection)
        {
            connection = Connection;
			code = new OK();
            handler = null;
            obj_request = null;
            reads_bytes = null;
        }

        public void Execute() {
            byte[] new_line = new byte[] { 13, 10, 13, 10 };
            List<byte> data = new List<byte>();
            byte[] buffer = new byte[1024];
			int count = 0;
            int index = 0;
			while ((count = connection.GetStream().Read(buffer, 0, buffer.Length)) > 0) {
                data.AddRange(buffer.Take(count));
                if (ExistSeqeunce(new_line, buffer, out index)) { //Запрос обрывается \r\n\r\n последовательностью
                    index += (data.Count - count);
                    break;
                }
            }
			try {
				obj_request = Reqest.CreateNewReqest(data, index, connection);
				obj_request.CheckTabelOfRedirect();
				reads_bytes = new Reader(obj_request);
			}
			catch (ExceptionCode err) {
				code = err;
			}
			catch (Exception err2) {
				code = Repository.ExceptionFabrics["Internal Server Error"].Create(null);
			}
            response = new Response(code);
            try {
                byte[] send_data = response.GetData(obj_request, reads_bytes);
                connection.GetStream().Write(send_data, 0, send_data.Length);
            }
            catch (Exception err) {}
            finally {
                connection.Close();
				Repository.threads_count-=1;
				Console.WriteLine("закрытие соединения web server {0}\r\nthreads count : {1}", Repository.Configurate["name"].Value, Repository.threads_count);
            }
        }

        public static bool ExistSeqeunce(byte[] sequence, IEnumerable<byte> array, out int index) {
            int seq_i = 0;
            for (int i = 0; i < array.Count(); i++)
            {
                if (array.ElementAt(i) != sequence[seq_i]) {
                    i = i - seq_i;
                    seq_i = 0;
                }
                else {
                    if (seq_i == sequence.Length - 1) {
                        index = i - seq_i;
                        return true; 
                    }
                    seq_i++;
                }
            }
            index = -1;
            return false;
        }

    }
}
