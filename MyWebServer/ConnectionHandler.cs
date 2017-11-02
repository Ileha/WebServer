using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using MyWebServer.ServerExceptions;
using MyWebServer.HttpHandler;
using MyWebServer.MIME;

namespace MyWebServer {
    public class ConnectionHandler {
        public readonly Socket connection;
        public readonly WebSerwer ParentServer;

        public ExceptionCode code;
        public IHttpHandler handler;
        public Reader reads_bytes;
        public Reqest obj_request;
        public Response response;

        public ConnectionHandler(Socket Connection, WebSerwer myServer) {
            connection = Connection;
            ParentServer = myServer;
            code = ExceptionCode.OK();
            handler = null;
            obj_request = null;
            reads_bytes = null;
        }

        public void Execute() {
            byte[] buffer = new byte[1024];
            string request = "";
            while (true) {
                int bytesRec = connection.Receive(buffer);
                request += Encoding.UTF8.GetString(buffer, 0, bytesRec);
                if (request.IndexOf("\r\n\r\n") >= 0) { //Запрос обрывается \r\n\r\n последовательностью
                    break;
                }
            }

            try {
                obj_request = Reqest.CreateNewReqest(request, out handler);
                reads_bytes = new Reader(obj_request, ParentServer.workDirectory());
            }
            catch (ExceptionCode err) {
                code = err;
            }
            response = new Response(code, handler);
            //re.Write(Html);
            //Console.WriteLine(re.GetData());
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            //string Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" + Html;
            // Приведем строку к виду массива байт
            //Encoding.UTF8.GetBytes(re.GetData());
            // Отправим его клиенту
            connection.Send(response.GetData(this));
            // Закроем соединение
            connection.Close();
        }

        public IMIME GetIMIMEHandler(string extension) {
            Console.WriteLine("key "+extension);
            foreach (KeyValuePair<string, IMIME> k in MainProgramm.DataHandlers) {
                Console.WriteLine("{0} -> {1}", k.Key, k.Value);
            }
            return MainProgramm.DataHandlers[extension];
        }

    }
}
