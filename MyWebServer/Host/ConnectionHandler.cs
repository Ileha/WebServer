﻿using System;
using System.Net.Sockets;
using System.Text;
using Host.ServerExceptions;
using Config;
using Host.HttpHandler;
using Host.MIME;

namespace Host {
    public class ConnectionHandler {
        public readonly Socket connection;

        public ExceptionCode code;
        public IHttpHandler handler;
        public Reader reads_bytes;
        public Reqest obj_request;
        public Response response;

        public ConnectionHandler(Socket Connection) {
            connection = Connection;
			code = new OK();
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
            //Console.WriteLine(request);
            try {
                obj_request = Reqest.CreateNewReqest(request);
                obj_request.CheckTabelOfRedirect();
                reads_bytes = new Reader(obj_request);
            }
            catch (ExceptionCode err) {
                code = err;
            }
            response = new Response(code);
            connection.Send(response.GetData(obj_request, reads_bytes));
            connection.Close();
        }

    }
}
