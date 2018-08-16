using Configurate.Host.Connection.Exceptions;
using Configurate.Host.Connection.Reader;
using Configurate.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Host.Connection.HTTPConnection
{
    public class HttpEventConnection : IConnetion
    {
        private HTTPConnectionHandler handler;

        public HttpEventConnection(HTTPConnectionHandler MainConnection, TcpClient client) {
            handler = MainConnection;
            RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            LocalEndPoint = client.Client.LocalEndPoint as IPEndPoint;
        }
        public Stream InputData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }
        public Stream OutputData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }
        public UserConnect UserConnectData {
            get { 
                if (handler.UserData != null) {
                    return handler.UserData;
                }
                else {
                    throw new ConnectionExecutorBadAccess();
                }
            }
        }
        public Reader.Reader ReadData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }
        public string ConnectionType
        {
            get { return "http"; }
        }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
    }
}
