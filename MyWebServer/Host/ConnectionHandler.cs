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
using Host.Session;

namespace Host {
    public class ConnectionHandler {
        public readonly TcpClient connection;

        public ExceptionCode code;
        public IHttpHandler handler;
        public Reader reads_bytes;
        public Reqest obj_request;
        public Response response;
		public UserConnect UserData;
		public IMIME DataHandle;

        public ConnectionHandler(TcpClient Connection)
        {
            connection = Connection;
			code = new OK();
            handler = null;
            obj_request = null;
            reads_bytes = null;
			UserData = null;
			DataHandle = null;
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
			response = new Response(connection);
			try {
				obj_request = Reqest.CreateNewReqest(data, index, connection);//создание экземпляра класса запроса
				obj_request.CheckTabelOfRedirect();//проверка таблицы перенаправлений
				try {//попытка найти данные к запросу
					UserData = UserConnect.GetUserDataFromID(obj_request.cookies[Repository.Configurate["webserver"].Element("guid").Value.ToString()]);
                }
                catch(Exception err) {
					//при неудачной попытки(осутствуют данные или нет информации в куках) создать данные и запись куков в ответ
					UserData = new UserConnect();
					response.SetCookie(Repository.Configurate["webserver"].Element("guid").Value.ToString(), UserData.ID);
				}
				//нахождение пользователя

				reads_bytes = new Reader(obj_request);//нахождение и получение запрошенных данных
				try {//попытка найти обработчик данных
					DataHandle = Repository.DataHandlers[reads_bytes.file_extension];
				}
				catch (Exception err) {
					//при неудачной попытки бросаем исключение
					throw Repository.ExceptionFabrics["Internal Server Error"].Create(null);
				}
				DataHandle.Handle(ref response, ref obj_request, ref reads_bytes);//вызов обработчика данных
			}
			catch (ExceptionCode err) {
				code = err;
			}

			response.code = code;
			response.SendData(obj_request);

			Repository.threads_count-=1;
			Console.WriteLine("закрытие соединения web server {0}\r\nthreads count : {1}", Repository.Configurate["name"].Value, Repository.threads_count);
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
