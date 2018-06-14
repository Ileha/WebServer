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
    public class HttpEventConnection : IConnetion
    {
        private IConnetion main_connection;

        public HttpEventConnection(IConnetion MainConnection) {
            main_connection = MainConnection;
        }

        public Stream InputData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }

        public Stream OutputData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }

        public UserConnect UserConnectData
        {
            get { 
                if (main_connection.UserConnectData != null) {
                    return main_connection.UserConnectData;
                }
                else {
                    throw new ConnectionExecutorBadAccess();
                }
            }
        }

        public IReader ReadData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }

        public ConnectionType ConnectType
        {
            get { return main_connection.ConnectType; }
        }
    }
}
