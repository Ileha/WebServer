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
using Host.ConnectionHandlers.ExecutorExceptions;

namespace Host.ConnectionHandlers {
    public class ConnectionHandler : IConnectionHandler, IConnetion {
        private TcpClient connection;
        private IReader reads_bytes;
        private Reqest obj_request;
        private Response response;
        private UserConnect UserData;
        private ABSMIME DataHandle;
        private UserInfo User;
        private IConnectionHandler actual_handler;
        private Func<Stream> out_data;
        private Func<Stream> in_data;

		public TcpClient Client { get { return connection; } }

		public ConnectionHandler(TcpClient Connection) {
            connection = Connection;
        }

        public Stream InputData {
            get { return in_data(); }
        }

        public Stream OutputData {
			get { return out_data(); }
        }

        public UserConnect UserConnectData {
            get { return UserData; }
        }

        public IReader ReadData {
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


        public IConnectionHandler ExecuteHandler {
            get { return actual_handler; }
        }

        public void Execute() {
            actual_handler = this;
            response = Response.Create(Client, out out_data);//создание экземпляра класса ответа

            try {
                obj_request = Reqest.Create(Client, out in_data);//создание экземпляра класса запроса;
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
                        if (find.Password == decodedString[1]) {
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
                    DataHandle = Repository.DataHandlers[reads_bytes.FileExtension];
                }
                catch (Exception err) {//при неудачной попытки бросаем исключение
                    throw Repository.ExceptionFabrics["Not Implemented"].Create(null, null);
                }
                //websocket
                try {
                    if (obj_request.preferens["Upgrade"] == "websocket") {
                        actual_handler = new WebSocketHandler(Client, reads_bytes, UserData);
                        string[] data = new string[] { "websocket" };
                        throw Repository.ExceptionFabrics["Switching Protocols"].Create(
                            (handle, _request, _response, dat, func) => {
                                func(handle, _request, _response, dat);
                                string str = _request.preferens["Sec-WebSocket-Key"] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                                byte[] bytes = Encoding.UTF8.GetBytes(str);
                                var sha1 = SHA1.Create();
                                byte[] hashBytes = sha1.ComputeHash(bytes);
                                _response.AddToHeader("Sec-WebSocket-Accept", Convert.ToBase64String(hashBytes), AddMode.rewrite);
                            },
                        data);
                    }
                }
                catch (KeyNotFoundException err) { }
            }
            catch (ExceptionCode err) {
                response.SetCode(err);
            }
            response.code.ExceptionHandle(DataHandle, obj_request, response, this);
            response.SendData(obj_request);

            if (response.GetHeader("Connection") == "close") {
                throw new ConnectionExecutorClose();
            }
        }

        public void Dispose() {
            Client.GetStream().Dispose();
            Client.Close();
        }


        public void Reset()
        {
            if (reads_bytes != null) {
                reads_bytes.Dispose();
                reads_bytes = null;
            }
            DataHandle = null;
            response.Dispose();
            response = null;
            obj_request.Dispose();
            obj_request = null;
        }
    }
}
