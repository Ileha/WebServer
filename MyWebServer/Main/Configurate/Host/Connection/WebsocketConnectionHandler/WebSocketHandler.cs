using Configurate.Host.Connection.HTTPConnection;
using Configurate.Host.Connection.Reader;
using Configurate.Session;
using DataHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Host.Connection.WebsocketConnection
{
    public class WebSocketHandler : IConnectionHandler, IConnetion
    {
        private TcpClient client;
        private Reader.Reader reads_bytes;
        public ABSMIME DataHandle;
        public UserConnect UserData;
        private WebSocketStream SocketStream;
        private MemoryStream InputDataStream;
        private MemoryStream OutputDataStream;

        public WebSocketHandler(TcpClient client, Reader.Reader data, UserConnect user_data)
        {
            this.client = client;
            reads_bytes = data;
            DataHandle = Repository.DataHandlers[reads_bytes.FileExtension];
            UserData = user_data;
            SocketStream = new WebSocketStream(client.GetStream());
            InputDataStream = new MemoryStream();
            OutputDataStream = new MemoryStream();
        }

        public Stream InputData
        {
            get { return InputDataStream; }
        }

        public Stream OutputData
        {
            get { return OutputDataStream; }
        }

        public UserConnect UserConnectData
        {
            get { return UserData; }
        }

        public Reader.Reader ReadData
        {
            get { return reads_bytes; }
        }

        public string ConnectionType
        {
            get { return "websocket"; }
        }
        public IConnetion GetConnetion {
            get { return this; }
        }

        public IConnectionHandler ExecuteHandler
        {
            get { return this; }
        }

        public void Execute()
        {
            byte[] data = new byte[1024];
            do
            {
                int count = SocketStream.Read(data, 0, 1024);
                InputDataStream.Write(data, 0, count);
            } while (SocketStream.CanRead);
            InputDataStream.Seek(0, SeekOrigin.Begin);
            Action<string, string> add_headers = (name, value) => { };
            DataHandle.Handle(this, add_headers);
            OutputDataStream.Seek(0, SeekOrigin.Begin);
            OutputDataStream.CopyTo(SocketStream);
        }

        public void Dispose()
        {
            client.GetStream().Dispose();
            client.Close();
        }


        public void Reset() {
            InputDataStream = new MemoryStream();
            OutputDataStream = new MemoryStream();
            reads_bytes.Data.Seek(0, SeekOrigin.Begin);
        }

        public IConnetion GetEventConnetion
        {
            get { return new WebsocketConnection(this); }
        }
    }
}
