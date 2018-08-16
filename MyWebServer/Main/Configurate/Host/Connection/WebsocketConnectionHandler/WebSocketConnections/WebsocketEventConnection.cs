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


namespace Configurate.Host.Connection.WebsocketConnection
{
    public class WebsocketEventConnection : IConnetion
    {
        private WebSocketHandler handler;

        public WebsocketEventConnection(WebSocketHandler main, TcpClient client)
        {
            handler = main;
            RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            LocalEndPoint = client.Client.LocalEndPoint as IPEndPoint;
        }

        public Stream InputData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }

        public Stream OutputData
        {
            get { return handler.SocketStream; }
        }

        public UserConnect UserConnectData
        {
            get { return handler.UserData; }
        }

        public Reader.Reader ReadData
        {
            get { return handler.ReadData; }
        }

        public string ConnectionType
        {
            get { return "websocket"; }
        }

        public IPEndPoint RemoteEndPoint { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
    }
}
