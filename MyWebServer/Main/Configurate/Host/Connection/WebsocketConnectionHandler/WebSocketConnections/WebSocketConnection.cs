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
    public class WebSocketConnection : IConnetion {
        private WebSocketHandler handler;
        public WebSocketConnection(WebSocketHandler main, TcpClient client) {
            handler = main;
            RemoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            LocalEndPoint = client.Client.LocalEndPoint as IPEndPoint;
        }

        public IPEndPoint RemoteEndPoint { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }

        public Stream InputData
        {
            get { return handler.InputDataStream; }
        }

        public Stream OutputData
        {
            get { return handler.OutputDataStream; }
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
    }
}
