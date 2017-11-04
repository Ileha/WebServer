using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using MyWebServer.ServerExceptions;
using MyWebServer.HttpHandler;
using MyWebServer.MIME;
using MyWebServer.WebServerConfigure;

namespace MyWebServer {
    public class ConnectionHandler {
        public readonly Socket connection;
        public readonly IConfigRead ParentServerConfig;
        public readonly IConfigRead RedirectTable;

        public ExceptionCode code;
        public IHttpHandler handler;
        public Reader reads_bytes;
        public Reqest obj_request;
        public Response response;

        public ConnectionHandler(Socket Connection, WebServerConfig myServerConfig) {
            connection = Connection;
            ParentServerConfig = myServerConfig;
            RedirectTable = myServerConfig.RedirectConfigure;
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
            Console.WriteLine(request);
            try {
                obj_request = Reqest.CreateNewReqest(request, GetHTTPHandler, ParentServerConfig);
                reads_bytes = new Reader(obj_request, ParentServerConfig);
            }
            catch (ExceptionCode err) {
                code = err;
            }
            response = new Response(code);
            connection.Send(response.GetData(GetIMIMEHandler, obj_request, reads_bytes, ParentServerConfig));
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
