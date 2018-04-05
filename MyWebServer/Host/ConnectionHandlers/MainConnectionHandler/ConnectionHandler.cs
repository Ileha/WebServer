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
using System.Security.Cryptography;
using System.IO;

namespace Host.ConnectionHandlers {
    public class ConnectionHandler : IConnectionHandler, IConnetion {
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
            code = Repository.ExceptionFabrics["OK"].Create(null, null);
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
            code = Repository.ExceptionFabrics["OK"].Create(null, null);
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
						byte[] data_authentication = Convert.FromBase64String(Regex.Split(obj_request.preferens["Authorization"], " ")[1]);
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

                reads_bytes = new Reader(obj_request.URL, User);//нахождение и получение запрошенных данных
                try {//попытка найти обработчик данных
                    DataHandle = Repository.DataHandlers[reads_bytes.file_extension];
                }
                catch (Exception err) {//при неудачной попытки бросаем исключение
                    throw Repository.ExceptionFabrics["Internal Server Error"].Create(null, null);
                }
				//websocket
                try {
                    if (obj_request.preferens["Upgrade"] == "websocket") {
                        res = new WebSocketHandler(Client, reads_bytes, UserData);
                        string[] data = new string[] { "websocket" };
                        throw Repository.ExceptionFabrics["Switching Protocols"].Create(
                            (ref Reqest _request, ref Response _response) => {
                                string str = _request.preferens["Sec-WebSocket-Key"] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                                byte[] bytes = Encoding.UTF8.GetBytes(str);
                                var sha1 = SHA1.Create();
                                byte[] hashBytes = sha1.ComputeHash(bytes);
                                _response.AddToHeader("Sec-WebSocket-Accept", Convert.ToBase64String(hashBytes), AddMode.rewrite);
                            },
                        data);
                    }
                }
                catch (KeyNotFoundException err) {}

                DataHandle.Headers(ref response, ref obj_request, ref reads_bytes);//вызов обработчика данных для заголовков
                IConnetion this_connection = this;
                DataHandle.Handle(ref this_connection);
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

        public Stream InputData {
            get { return obj_request.Data; }
        }

        public Stream OutputData {
			get { return response.DataWriter; }
        }

        public UserConnect UserConnectData {
            get { return UserData; }
        }

        public Reader ReadData {
            get { return reads_bytes; }
        }

        public ConnectionType ConnectType {
            get {
                return ConnectionType.http;
            }
        }

        public IConnetion GetConnetion
        {
            get { return this; }
        }
    }
}
