using System;
using System.Net;
using System.Net.Sockets;
using Config;
using System.Threading.Tasks;
using System.Collections.Generic;
using Host.HttpHandler;
using Host.MIME;
using System.Linq;
using System.Xml.Linq;
using Host.ServerExceptions;
using Host.ConnectionHandlers;
using Host.Eventer;

namespace Host {
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
            if (Repository.DirReader != null && Repository.DirReader.GetType().GetInterfaces().Contains(target)) {
                AddEvents(Repository.DirReader as IHostEvents);
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
            foreach (IGrub eve in Repository.Eventers) {
                AddEvents(eve);
            }
        }

        private void AddEvents(IHostEvents listner) {
            onStartHost += listner.OnStart;
            onStopHost += listner.OnStop;
        }

        private void ThreadFunc() {
            try {
                InvokeStart(this);
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }
            try {
                sListener.Start();
				Console.WriteLine("Запуск сервера {0}", Repository.ConfigBody.Element("name").Value);
                while (is_work) {
                    //Программа приостанавливается, ожидая входящее соединение
                    TcpClient handler = sListener.AcceptTcpClient();
					ConnectionExecutor executor = new ConnectionExecutor(new ConnectionHandler(handler));
					Console.WriteLine("хост {1}, новое соединение через порт {0}", ipEndPoint, Repository.ConfigBody.Element("name").Value);
                    Task handle = new Task(executor.Execute);
                    handle.Start();
                }
            }
            catch (Exception err) {
                Console.WriteLine(err.ToString());
            }
            finally {
                try {
                    InvokeStop(this);
                }
                catch (Exception err) { Console.WriteLine(err.ToString()); }
            }
        }

        private void InvokeStart(WebSerwer host) {
            if (onStartHost != null) {
                onStartHost(host);
            }
        }
        private void InvokeStop(WebSerwer host) {
            if (onStopHost != null) {
                onStopHost(host);
            }
        }

		public void Configurate(XElement data) {
			IPAddress adres = null;
			if (data.Element("ip_adress").Value != "+") {
				adres = IPAddress.Parse(data.Element("ip_adress").Value);
			}
			else {
				adres = IPAddress.Any;
			}
			ipEndPoint = new IPEndPoint(adres, Convert.ToInt32(data.Element("port").Value));
			data.Add(new XElement("guid", Guid.NewGuid().ToString("N")));
            sListener = new TcpListener(ipEndPoint);
			is_work = true;
			ConfigureEvents();
			thread = new Task(ThreadFunc);
		}
	}
}
