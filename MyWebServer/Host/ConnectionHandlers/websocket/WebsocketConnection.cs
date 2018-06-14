﻿using Host.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.ConnectionHandlers.ExecutorExceptions;


namespace Host.ConnectionHandlers
{
    public class WebsocketConnection : IConnetion
    {
        private IConnetion main_connection;

        public WebsocketConnection(IConnetion main) {
            main_connection = main;
        }

        public Stream InputData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }

        public Stream OutputData
        {
            get { return main_connection.OutputData; }
        }

        public UserConnect UserConnectData
        {
            get { return main_connection.UserConnectData; }
        }

        public IReader ReadData
        {
            get { return main_connection.ReadData; }
        }

        public ConnectionType ConnectType
        {
            get { return main_connection.ConnectType; }
        }
    }
}
