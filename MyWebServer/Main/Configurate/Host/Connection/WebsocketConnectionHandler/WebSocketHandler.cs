using Configurate.Host.Connection.HTTPConnection;
using Configurate.Host.Connection.Reader;
using Configurate.Session;
using DataHandlers;
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
    public class WebSocketHandler : IConnectionHandler
    {
        private TcpClient client;
        private ABSMIME DataHandle;

        public Reader.Reader ReadData { get; private set; }
        public UserConnect UserData { get; private set; }
        public MemoryStream InputDataStream { get; private set; }
        public MemoryStream OutputDataStream { get; private set; }
        public WebSocketStream SocketStream { get; private set; }

        public WebSocketHandler(TcpClient client, Reader.Reader data, UserConnect user_data) {
            this.client = client;
            ReadData = data;
            DataHandle = Repository.DataHandlers[ReadData.FileExtension];
            UserData = user_data;
            SocketStream = new WebSocketStream(client.GetStream());
            GetEventConnetion = new WebsocketEventConnection(this, this.client);
            GetConnetion = new WebSocketConnection(this, this.client);
        }
        public IConnetion GetConnetion { get; private set; }
        public IConnetion GetEventConnetion { get; private set; }
        public IConnectionHandler ExecuteHandler {
            get { return this; }
        }

        public void Execute() {
            InputDataStream = new MemoryStream();
            OutputDataStream = new MemoryStream();
            ReadData.Data.Seek(0, SeekOrigin.Begin);

            byte[] data = new byte[1024];
            do {
                int count = SocketStream.Read(data, 0, 1024);
                InputDataStream.Write(data, 0, count);
            } while (SocketStream.CanRead);
            InputDataStream.Seek(0, SeekOrigin.Begin);
            Action<string, string> add_headers = (name, value) => { };
            try {
                DataHandle.Handle(GetConnetion, add_headers);
            }catch(Exception err) {
                byte[] data_err = Encoding.UTF8.GetBytes(err.ToString());
                OutputDataStream.Write(data, 0, data.Length);
            }

            OutputDataStream.Seek(0, SeekOrigin.Begin);
            OutputDataStream.CopyTo(SocketStream);
        }
        public void Dispose() {
            try
            {
                client.Close();
            }
            catch (Exception err) { }
        }
    }
}
