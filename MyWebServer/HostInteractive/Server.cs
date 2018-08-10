using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Assets;
using System.Threading;

namespace HostInteractive
{
    public class Server
    {
        private Action<object> ActionOnConnect;
        private Task work;
        public int Port { get; private set; }

        public Server(Action<object> ActionOnConnect) {
            this.ActionOnConnect = ActionOnConnect;
            Port = MyRandom.rnd.Next(49152, 65535);
            work = new Task(ServerFunc);
            work.Start();
        }

        private void ServerFunc()
        {

            IPAddress adres = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(adres, Port);

            TcpListener Listener = new TcpListener(ipEndPoint);
            Listener.Start();

            try
            {

                while (true) {
                    TcpClient handler = Listener.AcceptTcpClient();
                    Task.Factory.StartNew(ActionOnConnect, new StringOIInterface(handler));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
