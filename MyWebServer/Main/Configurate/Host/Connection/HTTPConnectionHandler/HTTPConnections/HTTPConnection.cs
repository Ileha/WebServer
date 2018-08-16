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
    public class HTTPConnection : IConnetion {
        private HTTPConnectionHandler handler;
        public HTTPConnection(HTTPConnectionHandler handler, TcpClient client) {
            RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            LocalEndPoint = client.Client.LocalEndPoint as IPEndPoint;
            this.handler = handler;
        }

        public IPEndPoint RemoteEndPoint { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
        public Stream InputData { get { return handler.InData; } }
        public Stream OutputData { get { return handler.OutData; } }
        public UserConnect UserConnectData { get { return handler.UserData; } }
        public Reader.Reader ReadData { get { return handler.reads_bytes; } }

        public string ConnectionType
        {
            get { return "http"; }
        }
    }
}
