using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace HostInteractive
{
    public class Client
    {
        private Task work;
        private Action<StringOIInterface> ActionOnConnect;
        public int Port { get; private set; }

        public Client(int Port, Action<StringOIInterface> ActionOnConnect)
        {
            this.Port = Port;
            this.ActionOnConnect = ActionOnConnect;
            work = new Task(ClientFunc);
            work.Start();
        }

        private void ClientFunc() {
            //IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            //IPAddress ipAddr = ipHost.AddressList[0];
            //IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, Port);
            //TcpClient client = new TcpClient(ipEndPoint);
            TcpClient client = new TcpClient("127.0.0.1", Port);
            //client.Connect(ipEndPoint);
            ActionOnConnect(new StringOIInterface(client));
        }
    }
}
