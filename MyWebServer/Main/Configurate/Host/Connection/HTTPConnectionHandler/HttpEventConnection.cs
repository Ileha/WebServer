using Configurate.Host.Connection.Exceptions;
using Configurate.Host.Connection.Reader;
using Configurate.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Host.Connection.HTTPConnection
{
    public class HttpEventConnection : IConnetion
    {
        private IConnetion main_connection;

        public HttpEventConnection(IConnetion MainConnection) {
            main_connection = MainConnection;
        }

        public Stream InputData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }

        public Stream OutputData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }

        public UserConnect UserConnectData
        {
            get { 
                if (main_connection.UserConnectData != null) {
                    return main_connection.UserConnectData;
                }
                else {
                    throw new ConnectionExecutorBadAccess();
                }
            }
        }

        public Reader.Reader ReadData
        {
            get { throw new ConnectionExecutorBadAccess(); }
        }

        public string ConnectionType
        {
            get { return "http"; }
        }
    }
}
