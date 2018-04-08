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

		private event HostEvent onConnect;
		private event HostEvent onDisConnect;

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
            Type onStart = typeof(OnWebServerStart);
			Type onStop = typeof(OnWebServerStop);
   //         if (Repository.DirReader != null && Repository.DirReader.GetType().GetInterfaces().Contains(onStart)) {;
			//	onStartHost += (Repository.DirReader as OnWebServerStart).OnStart;
   //         }
			//if (Repository.DirReader != null && Repository.DirReader.GetType().GetInterfaces().Contains(onStop)) {
			//	onStartHost += (Repository.DirReader as OnWebServerStop).OnStop;
   //         }

            foreach (KeyValuePair<string, IHttpHandler> el in Repository.ReqestsHandlers) {
                if (el.Value.GetType().GetInterfaces().Contains(onStart)) {
                    onStartHost += (el.Value as OnWebServerStart).OnStart;
                }
				if (el.Value.GetType().GetInterfaces().Contains(onStop)) {
					onStartHost += (el.Value as OnWebServerStop).OnStop;
                }
            }
            foreach (KeyValuePair<string, IMIME> el in Repository.DataHandlers) {
                if (el.Value.GetType().GetInterfaces().Contains(onStart)) {
                    onStartHost += (el.Value as OnWebServerStart).OnStart;
                }
				if (el.Value.GetType().GetInterfaces().Contains(onStop)) {
					onStartHost += (el.Value as OnWebServerStop).OnStop;
                }
            }
			foreach (KeyValuePair<string, ExceptionFabric> el in Repository.ExceptionFabrics) {
                if (el.Value.GetType().GetInterfaces().Contains(onStart)) {
                    onStartHost += (el.Value as OnWebServerStart).OnStart;
                }
				if (el.Value.GetType().GetInterfaces().Contains(onStop)) {
					onStartHost += (el.Value as OnWebServerStop).OnStop;
                }
            }
            foreach (IGrub eve in Repository.Eventers) {
				onStartHost += eve.OnStart;
				onStopHost += eve.OnStop;
				onConnect += eve.OnConntect;
				onDisConnect += eve.OnDisConntect;
            }
        }

        private void ThreadFunc() {
			HostEventData data = new HostEventData(this);
            try {
                InvokeStart(data);
            }
            catch (Exception err) { Console.WriteLine(err.ToString()); }
            try {
                sListener.Start();
				Console.WriteLine("Запуск сервера {0}", Repository.ConfigBody.Element("name").Value);
                while (is_work) {
                    //Программа приостанавливается, ожидая входящее соединение
                    TcpClient handler = sListener.AcceptTcpClient();
					ConnectionExecutor executor = new ConnectionExecutor(new ConnectionHandler(handler), onConnect, onDisConnect);
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
                    InvokeStop(data);
                }
                catch (Exception err) { Console.WriteLine(err.ToString()); }
            }
        }

        private void InvokeStart(HostEventData host) {
            if (onStartHost != null) {
                onStartHost(host);
            }
        }
        private void InvokeStop(HostEventData host) {
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
