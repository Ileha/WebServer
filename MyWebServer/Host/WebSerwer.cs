using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using Config;
using System.Threading.Tasks;
using System.Collections.Generic;
using Host.HttpHandler;
using Host.MIME;
using System.Linq;
using System.Xml.Linq;
using Host.ServerExceptions;

namespace Host {
    public delegate void HandlerExecutor();

    public class WebSerwer : IConfigurate {
        private TcpListener sListener;
        private IPEndPoint ipEndPoint;
        private bool is_work;
		private Task thread;

        public event HostEvent onStartHost;
        public event HostEvent onStopHost;

        private string[] names = new string[] { "webserver" };
		public string[] ConfigName {
			get {
                return names;
			}
		}

		public WebSerwer() {}

		public void Start() {
			thread.Start();
		}

        public void ConfigureEvents() {
            Type target = typeof(IHostEvents);
            if (Repository.Configurate.DirReader != null && Repository.Configurate.DirReader.GetType().GetInterfaces().Contains(target)) {
                AddEvents(Repository.Configurate.DirReader as IHostEvents);
            }
            foreach (KeyValuePair<string, IHttpHandler> el in Repository.ReqestsHandlers) {
                if (el.Value.GetType().GetInterfaces().Contains(target)) {
                    AddEvents(el.Value as IHostEvents);
                }
            }
            foreach (KeyValuePair<string, IMIME> el in Repository.DataHandlers) {
                if (el.Value.GetType().GetInterfaces().Contains(target)) {
                    AddEvents(el.Value as IHostEvents);
                }
            }
			foreach (KeyValuePair<string, ExceptionFabric> el in Repository.ExceptionFabrics) {
                if (el.Value.GetType().GetInterfaces().Contains(target)) {
					AddEvents(el.Value as IHostEvents);
                }
            }
        }

        private void AddEvents(IHostEvents listner) {
            onStartHost += listner.OnStart;
            onStopHost += listner.OnStop;
        }

        private void ThreadFunc() {
            try {
                InvokeStart();
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }
            try {
                sListener.Start();
				Console.WriteLine("Запуск сервера {0}", Repository.Configurate.ConfigBody.Element("name").Value);
                while (is_work) {
                    // Программа приостанавливается, ожидая входящее соединение
                    TcpClient handler = sListener.AcceptTcpClient();
					#if DEBUG
						Console.WriteLine("хост {1}, соединение через порт {0}", ipEndPoint, Repository.Configurate.ConfigBody.Element("name").Value);
					#endif
					Repository.threads_count+=1;
                    ConnectionHandler executor = new ConnectionHandler(handler);
                    Task handle = new Task(executor.Execute);
                    handle.Start();
                }
            }
            catch (Exception err) {
                Console.WriteLine(err.ToString());
            }
            finally {
                try {
                    InvokeStop();
                }
                catch (Exception err) { Console.WriteLine(err.ToString()); }
            }
        }

        private void InvokeStart() {
            if (onStartHost != null) {
                onStartHost();
            }
        }
        private void InvokeStop() {
            if (onStopHost != null) {
                onStopHost();
            }
        }

		public void Configurate(XElement data) {
			IPAddress adres = IPAddress.Parse(data.Element("ip_adress").Value);
			ipEndPoint = new IPEndPoint(adres, Convert.ToInt32(data.Element("port").Value));
			data.Add(new XElement("guid", Guid.NewGuid().ToString("N")));
            sListener = new TcpListener(ipEndPoint);
			is_work = true;
			ConfigureEvents();
			thread = new Task(ThreadFunc);
		}
	}
}
