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
using System.Threading;

namespace Host {
    public class WebSerwer : IConfigurate, IDisposable {
        private TcpListener sListener;
        private IPEndPoint ipEndPoint;
        private bool is_work;
        private Thread thread;

        public event HostEvent onStartHost;
        public event HostEvent onStopHost;

		private event ConnectionEvent onConnect;
		private event ConnectionEvent onDisConnect;

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
			Type tonConnect = typeof(OnWebServerConntect);
			Type tonDisconnect = typeof(OnWebServerDisConntect);

            foreach (KeyValuePair<string, ABSHttpHandler> el in Repository.ReqestsHandlers) {
                if (el.Value.GetType().GetInterfaces().Contains(onStart)) {
                    onStartHost += (el.Value as OnWebServerStart).OnStart;
                }
				if (el.Value.GetType().GetInterfaces().Contains(onStop)) {
					onStopHost += (el.Value as OnWebServerStop).OnStop;
                }
				if (el.Value.GetType().GetInterfaces().Contains(tonConnect)) {
                    onConnect += (el.Value as OnWebServerConntect).OnConntect;
                }
				if (el.Value.GetType().GetInterfaces().Contains(tonDisconnect)) {
					onDisConnect += (el.Value as OnWebServerDisConntect).OnDisConntect;
                }
            }
            foreach (KeyValuePair<string, ABSMIME> el in Repository.DataHandlers) {
                if (el.Value.GetType().GetInterfaces().Contains(onStart)) {
                    onStartHost += (el.Value as OnWebServerStart).OnStart;
                }
				if (el.Value.GetType().GetInterfaces().Contains(onStop)) {
					onStopHost += (el.Value as OnWebServerStop).OnStop;
                }
				if (el.Value.GetType().GetInterfaces().Contains(tonConnect)) {
                    onConnect += (el.Value as OnWebServerConntect).OnConntect;
                }
				if (el.Value.GetType().GetInterfaces().Contains(tonDisconnect)) {
					onDisConnect += (el.Value as OnWebServerDisConntect).OnDisConntect;
                }
            }
			foreach (KeyValuePair<string, ABSExceptionFabric> el in Repository.ExceptionFabrics) {
                if (el.Value.GetType().GetInterfaces().Contains(onStart)) {
                    onStartHost += (el.Value as OnWebServerStart).OnStart;
                }
				if (el.Value.GetType().GetInterfaces().Contains(onStop)) {
					onStopHost += (el.Value as OnWebServerStop).OnStop;
                }
				if (el.Value.GetType().GetInterfaces().Contains(tonConnect)) {
                    onConnect += (el.Value as OnWebServerConntect).OnConntect;
                }
				if (el.Value.GetType().GetInterfaces().Contains(tonDisconnect)) {
					onDisConnect += (el.Value as OnWebServerDisConntect).OnDisConntect;
                }
            }
            foreach (ABSGrub eve in Repository.Eventers) {
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
					Console.WriteLine("хост {2}, новое соединение через {0}:{1}", ipEndPoint.Address, ipEndPoint.Port, Repository.ConfigBody.Element("name").Value);
                    Task handle = new Task(executor.Execute);
                    handle.Start();
                }
            }
            catch (ThreadAbortException Abort) {
                Console.WriteLine("Остановка сервера {0}", Repository.ConfigBody.Element("name").Value);
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
            thread = new Thread(ThreadFunc) { IsBackground = true, Priority = ThreadPriority.Highest };
		}

        public void Dispose() {
            if (!is_work) { return; }
            is_work = false;
            sListener.Stop();
            thread.Abort();
            thread.Join();
        }
    }
}
