using System;
using System.Net.Sockets;
using Host.ServerExceptions;
using Config;
using Host.HttpHandler;
using Host.MIME;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Host.Session;
using Host.Users;
using System.Text;
using System.Text.RegularExpressions;

namespace Host.ConnectionHandlers {
    public class ConnectionHandler : IConnectionHandler {
        public readonly TcpClient connection;

        public ExceptionCode code;
        public IHttpHandler handler;
        public Reader reads_bytes;
        public Reqest obj_request;
        public Response response;
		public UserConnect UserData;
		public IMIME DataHandle;
		public UserInfo User;
		public TcpClient Client { get { return connection; } }

		public ConnectionHandler(TcpClient Connection)
        {
            connection = Connection;
			code = new OK();
            handler = null;
            obj_request = new Reqest(connection);//создание экземпляра класса запроса;
            reads_bytes = null;
			UserData = null;
			DataHandle = null;
			response = new Response(connection); //создание экземпляра класса ответа
			User = null;
        }

		public void Clear() {
			obj_request.Clear();
			response.Clear();
			handler = null;
			code = new OK();
			reads_bytes = null;
			DataHandle = null;
		}

		public IConnectionHandler ExecuteHandler() //null on connection close
        {   
			IConnectionHandler res = this;
            try {
				obj_request.Create();
                obj_request.CheckTabelOfRedirect();//проверка таблицы перенаправлений
                try {//попытка найти данные к запросу
                    UserData = UserConnect.GetUserDataFromID(obj_request.cookies[Repository.ConfigBody.Element("webserver").Element("guid").Value.ToString()]);
                }
                catch (Exception err) {//при неудачной попытки(осутствуют данные или нет информации в куках) создать данные и запись куков в ответ
                    UserData = new UserConnect();
                    response.SetCookie(Repository.ConfigBody.Element("webserver").Element("guid").Value.ToString(), UserData.ID);
                }

				//нахождение пользователя
				do {
					try {
						byte[] data_authentication = Convert.FromBase64String(obj_request.preferens["Authorization"].Value[0].Value["1"]);
						string[] decodedString = Regex.Split(Encoding.UTF8.GetString(data_authentication), ":");
						UserInfo find = Repository.Configurate.Users.GetUserByName(decodedString[0]);
						if (find.Password == decodedString[1])
						{
							User = find;
							UserData.AddData("user", User);
							break;
						}
					}
					catch (Exception err) { }

					try {
						User = UserData.GetData<UserInfo>("user");
						break;
					}
					catch (Exception err) { }

					User = Repository.Configurate.Users.DefaultUser;
					UserData.AddData("user", User);
				} while (false);

                reads_bytes = new Reader(obj_request, User);//нахождение и получение запрошенных данных
                try {//попытка найти обработчик данных
                    DataHandle = Repository.DataHandlers[reads_bytes.file_extension];
                }
                catch (Exception err) {//при неудачной попытки бросаем исключение
                    throw Repository.ExceptionFabrics["Internal Server Error"].Create(null);
                }
				//websocket
				//GET /chat HTTP/1.1
				//Host: server.example.com
				//Upgrade: websocket
				//Connection: Upgrade
				//try {
				//	if (obj_request.preferens["Upgrade"].Value[0].Value["0"] == "websocket") {
						
				//	}
				//}
				//catch (Exception err) {
					
				//}

                DataHandle.Handle(ref response, ref obj_request, ref reads_bytes);//вызов обработчика данных
            }
            catch (ExceptionCode err) {
                code = err;
            }

            response.code = code;
			if (response.SendData(obj_request)) {
				return res;
			}
			else {
				return null;
			}

            //Repository.threads_count -= 1;
            //Console.WriteLine("закрытие соединения web server {0}\r\nthreads count : {1}", Repository.ConfigBody.Element("name").Value, Repository.threads_count);
        }
	}
}
