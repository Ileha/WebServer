using Host.MIME;
using Host.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Host.ConnectionHandlers
{
    class WebSocketHandler : IConnectionHandler, IConnetion
    {
        private TcpClient client;
        private Reader reads_bytes;
        public IMIME DataHandle;
        public UserConnect UserData;

        public WebSocketHandler(TcpClient client, Reader data, UserConnect user_data) {
            this.client = client;
            reads_bytes = data;
            DataHandle = Repository.DataHandlers[reads_bytes.file_extension];
            UserData = user_data;
        }

        public IConnectionHandler ExecuteHandler() {
            IConnectionHandler res = this;
            IConnetion this_connection = this;
            DataHandle.Handle(ref this_connection);
            return res;
        }

        public TcpClient Client {
            get { return client; }
        }

        public void Clear() {}

        public Stream InputData {
            get { return client.GetStream(); }
        }

        public Stream OutputData {
            get { return client.GetStream(); }
        }

        public UserConnect UserConnectData {
            get { return UserData; }
        }

        public Reader ReadData {
            get { return reads_bytes; }
        }
    }
}
