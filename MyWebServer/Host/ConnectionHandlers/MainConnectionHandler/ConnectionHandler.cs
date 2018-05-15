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

        private ExceptionCode code;
        private Reader reads_bytes;
        private Reqest obj_request;
        private Response response;
        private UserConnect UserData;
        private IMIME DataHandle;
        private UserInfo User;
        private IConnectionHandler actual_handler;
		public TcpClient Client { get { return connection; } }
		private Stream _outputData;

		public ConnectionHandler(TcpClient Connection) {
            connection = Connection;
            code = Repository.ExceptionFabrics["OK"].Create(null, null);
			_outputData = new MemoryStream();
        }

        public Stream InputData {
            get { return obj_request.Data; }
        }

        public Stream OutputData {
			get { return _outputData; }
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


        public IConnectionHandler ExecuteHandler {
            get { return actual_handler; }
        }

        public void Execute() {
            actual_handler = this;
            response = new Response(); //создание экземпляра класса ответа

            try {
                obj_request = new Reqest(Client);//создание экземпляра класса запроса;
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
                    DataHandle = Repository.DataHandlers[reads_bytes.file_extension];
                }
                catch (Exception err) {//при неудачной попытки бросаем исключение
                    throw Repository.ExceptionFabrics["Internal Server Error"].Create(null, null);
                }
                //websocket
                try {
                    if (obj_request.preferens["Upgrade"] == "websocket") {
                        actual_handler = new WebSocketHandler(Client, reads_bytes, UserData);
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
                catch (KeyNotFoundException err) { }
                DataHandle.Headers(ref response, ref obj_request, ref reads_bytes);//вызов обработчика данных для заголовков
                try {
                    IConnetion conn = this;
                    DataHandle.Handle(ref conn);//вызов обработчика данных
                }
                catch (Exception err) {
                    response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
                    byte[] exce = Encoding.UTF8.GetBytes(err.ToString());
                    OutputData.Write(exce, 0, exce.Length);
                }
            }
            catch (ExceptionCode err) {
                code = err;
            }

            response.code = code;
            response.SendData(obj_request, this, connection.GetStream());

            if (response.GetHeader("Connection") == "close") {
                throw new ConnectionExecutorClose();
            }
            Clear();
        }

        private void Clear() {
            code = Repository.ExceptionFabrics["OK"].Create(null, null);
            if (reads_bytes != null) {
                reads_bytes.Dispose();
                reads_bytes = null;
            }
            DataHandle = null;
            if (_outputData != null) {
                _outputData.Dispose();
            }
            _outputData = new MemoryStream();
            if (InputData != null) { InputData.Dispose(); }
            response = null;
            obj_request = null;
            UserData = null;
            User = null;
        }

        public void Dispose() {
            if (reads_bytes != null) { reads_bytes.Dispose(); }
            if (_outputData != null) { _outputData.Dispose(); }
            if (InputData != null) { InputData.Dispose(); }
            Client.GetStream().Dispose();
            Client.Close();
        }
    }
}
