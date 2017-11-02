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
                obj_request = Reqest.CreateNewReqest(request, GetHTTPHandler);
                reads_bytes = new Reader(obj_request, ParentServer.workDirectory());
            }
            catch (ExceptionCode err) {
                code = err;
            }
            response = new Response(code);
            connection.Send(response.GetData(GetIMIMEHandler, obj_request, reads_bytes));
            connection.Close();
        }

        public IMIME GetIMIMEHandler(string extension) {
            return MainProgramm.DataHandlers[extension];
        }
        public IHttpHandler GetHTTPHandler(TypeReqest RqType) {
            return MainProgramm.ReqestsHandlers[RqType];
        }

    }
}
