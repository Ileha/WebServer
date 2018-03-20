using Host.MIME;
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
        private Reader reads_bytes;
        public IMIME DataHandle;

        public WebSocketHandler(TcpClient client, Reader data) {
            this.client = client;
            reads_bytes = data;
            DataHandle = Repository.DataHandlers[reads_bytes.file_extension];
        }

        public IConnectionHandler ExecuteHandler() {
            IConnectionHandler res = this;

            return res;
        }

        public TcpClient Client {
            get { return client; }
        }

        public void Clear() {}
    }
}
