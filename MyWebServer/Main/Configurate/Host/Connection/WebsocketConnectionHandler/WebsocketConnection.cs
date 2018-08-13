using Configurate.Host.Connection.Exceptions;
using Configurate.Host.Connection.Reader;
using Configurate.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Configurate.Host.Connection.WebsocketConnection
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

        public Reader.Reader ReadData
        {
            get { return main_connection.ReadData; }
        }

        public string ConnectionType
        {
            get { return "websocket"; }
        }
    }
}
