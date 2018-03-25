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
    public class WebSocketHandler : IConnectionHandler, IConnetion
    {
        private TcpClient client;
        private Reader reads_bytes;
        public IMIME DataHandle;
        public UserConnect UserData;
		private WebSocketStream SocketStream;

        public WebSocketHandler(TcpClient client, Reader data, UserConnect user_data) {
            this.client = client;
            reads_bytes = data;
            DataHandle = Repository.DataHandlers[reads_bytes.file_extension];
            UserData = user_data;
			SocketStream = new WebSocketStream(client.GetStream());
        }

        public IConnectionHandler ExecuteHandler() {
            IConnectionHandler res = this;
			byte[] data = new byte[1024];
			string str = "";
			try {
				do {
					int count = SocketStream.Read(data, 0, 1024);
					str += Encoding.UTF8.GetString(data, 0, count);
				} while (SocketStream.CanRead);
				IConnetion this_connection = this;
				//DataHandle.Handle(ref this_connection);
				byte[] h = Encoding.UTF8.GetBytes(str);
				OutputData.Write(h, 0, h.Length);
			}
			catch (BreakConnection err) {
				res = null;
			}


            return res;
        }

        public TcpClient Client {
            get { return client; }
        }

        public void Clear() {}

        public Stream InputData {
			get { return SocketStream; }
        }

        public Stream OutputData {
            get { return SocketStream; }
        }

        public UserConnect UserConnectData {
            get { return UserData; }
        }

        public Reader ReadData {
            get { return reads_bytes; }
        }
    }
}
