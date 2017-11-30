using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using Config;
using System.Threading.Tasks;

namespace Host {
    public delegate void HandlerExecutor();

    public class WebSerwer {
        private TcpListener sListener;
        private IPEndPoint ipEndPoint;
        private bool is_work;

        public WebSerwer() {
            Task outer = Task.Factory.StartNew(() =>
            {
                Configure();
                var inner = Task.Factory.StartNew(ThreadFunc);
            });
        }

        public void Configure()
        {
            IPAddress adres = IPAddress.Parse(Repository.ReadConfig["ip_adress"]);
            ipEndPoint = new IPEndPoint(adres, Convert.ToInt32(Repository.ReadConfig["port"]));
            sListener = new TcpListener(ipEndPoint);
            is_work = true;
        }

        private void ThreadFunc() {
            try {
                sListener.Start();

                Console.WriteLine("Запуск сервера {0}", Repository.ReadConfig["name"]);
                while (is_work) {
                    // Программа приостанавливается, ожидая входящее соединение
                    TcpClient handler = sListener.AcceptTcpClient();
                    #if DEBUG
                        Console.WriteLine("хост {1}, соединение через порт {0}", ipEndPoint, Repository.ReadConfig["name"]);
                    #endif

                    ConnectionHandler executor = new ConnectionHandler(handler);
                    Task handle = new Task(executor.Execute);
                    handle.Start();
                }
            }
            catch (Exception err) {
                Console.WriteLine(err.ToString());
            }
        }
    }
}
