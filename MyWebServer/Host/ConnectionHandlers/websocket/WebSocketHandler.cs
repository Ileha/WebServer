using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Host.ConnectionHandlers
{
    class WebSocketHandler : IConnectionHandler
    {
        private TcpClient client;

        public WebSocketHandler(TcpClient client) {
            this.client = client;
        }

        public IConnectionHandler ExecuteHandler() {
            IConnectionHandler res = this;

            return res;
        }

        public TcpClient Client
        {
            get { return client; }
        }

        public void Clear() {}
    }
}
