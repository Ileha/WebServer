using System;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using Configurate.Host.Connection.Reader;
using Configurate.Session;
using DataHandlers;
using Configurate.Users;
using Configurate.Host.Connection.WebsocketConnection;
using ExceptionFabric;
using Configurate.Host.Connection.Exceptions;

namespace Configurate.Host.Connection.HTTPConnection
{
    public class HTTPConnectionHandler : IConnectionHandler {
        private TcpClient Client;
        private IConnectionHandler actual_handler;
        public Reader.Reader reads_bytes { get; private set; }
        public UserConnect UserData { get; private set; }
        public Stream OutData { get { return out_data(); } }
        public Stream InData { get { return in_data(); } }
        
        private Func<Stream> out_data;
        private Func<Stream> in_data;

        public HTTPConnectionHandler(TcpClient Connection) {
            Client = Connection;
            GetConnetion = new HTTPConnection(this, Client);
            GetEventConnetion = new HttpEventConnection(this, Client);
        }
        public IConnetion GetConnetion { get; private set; }
        public IConnetion GetEventConnetion { get; private set; }
        public IConnectionHandler ExecuteHandler {
            get { return actual_handler; }
        }

        public void Execute() {
            actual_handler = this;
            UserInfo User = null;
            Reqest request = null;
            Response response = Response.Create(Client, out out_data);//создание экземпляра класса ответа

            try {
                request = Reqest.Create(Client, out in_data);//создание экземпляра класса запроса;
                request.CheckTabelOfRedirect();//проверка таблицы перенаправлений 
                try {//попытка найти данные к запросу
                    UserData = UserConnect.GetUserDataFromID(request.cookies[Repository.Configurate.HostID.ToString()]);
                }
                catch (Exception err) {//при неудачной попытки(осутствуют данные или нет информации в куках) создать данные и запись куков в ответ
                    UserData = new UserConnect();
                    response.SetCookie(Repository.Configurate.HostID.ToString(), UserData.ID);
                }

                //нахождение пользователя
                do {
                    
                    try {
                        byte[] data_authentication = Convert.FromBase64String(Regex.Split(request.headers["Authorization"], " ")[1]);
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

                reads_bytes = new Reader.Reader(request.URL, User);//нахождение и получение запрошенных данных
                //websocket
                try {
                    if (request.headers["Upgrade"] == "websocket") {
                        actual_handler = new WebSocketHandler(Client, new Reader.Reader(reads_bytes), UserData);
                        string[] data = new string[] { "websocket" };
                        Action func = () =>
                        {
                            string str = request.headers["Sec-WebSocket-Key"] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                            byte[] bytes = Encoding.UTF8.GetBytes(str);
                            var sha1 = SHA1.Create();
                            byte[] hashBytes = sha1.ComputeHash(bytes);
                            response.AddToHeader("Sec-WebSocket-Accept", Convert.ToBase64String(hashBytes), AddMode.rewrite);
                        };
                        throw Repository.ExceptionFabrics["Switching Protocols"].Create(func, data);
                    }
                }
                catch (KeyNotFoundException err) { }
            }
            catch (ExceptionCode err) {
                response.SetCode(err);
            }

            bool is_handle_exception_success  = false;
            do {
                try
                {
                    response.code.ExceptionHandleCode(response, GetConnetion);
                    is_handle_exception_success = true;
                }
                catch (ExceptionCode ex_code) {
                    response.SetCode(ex_code);
                }
            } while (!is_handle_exception_success);

            response.SendData(request);

            if (reads_bytes != null) {
                reads_bytes.Dispose();
                reads_bytes = null;
            }

            if (response.GetHeader("Connection") == "close") {
                throw new ConnectionExecutorClose();
            }
        }
        public void Dispose() {
            try {
                Client.Close();
            }
            catch (Exception err) {}
        }
    }
}
