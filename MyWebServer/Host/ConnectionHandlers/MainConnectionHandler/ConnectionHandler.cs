﻿using System;
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
        private TcpClient connection;

        public ExceptionCode code;
        public Reader reads_bytes;
        public Reqest obj_request;
        public Response response;
		public UserConnect UserData;
		public IMIME DataHandle;
		public UserInfo User;
		public TcpClient Client { get { return connection; } }

		private Stream _outputData;

		public ConnectionHandler(TcpClient Connection) {
            connection = Connection;
            code = Repository.ExceptionFabrics["OK"].Create(null, null);
			_outputData = new MemoryStream();
        }

		public void Clear() {
            code = Repository.ExceptionFabrics["OK"].Create(null, null);
			reads_bytes.Dispose();
			reads_bytes = null;
			DataHandle = null;
			_outputData.Dispose();
			_outputData = new MemoryStream();
		}

		public IConnectionHandler ExecuteHandler() //null on connection close
        {   
			IConnectionHandler res = this;
			response = new Response(); //создание экземпляра класса ответа

            try {
				obj_request = new Reqest(connection.GetStream());//создание экземпляра класса запроса;
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
                            (ref Reqest _request, ref Response _response, IConnetion dat) => {
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
            }
            catch (ExceptionCode err) {
                code = err;
            }

			try {
				IConnetion conn = this;
				DataHandle.Handle(ref conn);
			}
			catch (Exception err) {
				response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
				byte[] exce = Encoding.UTF8.GetBytes(err.ToString());
				OutputData.Write(exce, 0, exce.Length);
			}

            response.code = code;
			response.SendData(obj_request, this, connection.GetStream());
			if (response.GetHeader("Connection") != "close") {
				return res;
			}
			else {
				connection.GetStream().Close();
				connection.GetStream().Dispose();
				return null;
			}
        }

        public Stream InputData {
            get { return obj_request.Data; }
        }

        public Stream OutputData {
			get { return _outputData; }
			set { _outputData = value; }
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
