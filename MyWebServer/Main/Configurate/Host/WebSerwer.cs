using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using RequestHandlers;
using DataHandlers;
using System.Linq;
using System.Xml.Linq;
using ExceptionFabric;
using Events;
using System.Threading;
using Configurate;
using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;

namespace Configurate.Host
{
    public class WebSerwer : IConfigurate, IDisposable {
        public TcpListener sListener { get; private set; }
        public IPEndPoint ipEndPoint { get; private set; }
        public bool IsWork { get; private set; }
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
                while (IsWork) {
                    //Программа приостанавливается, ожидая входящее соединение
                    TcpClient handler = sListener.AcceptTcpClient();
					ConnectionExecutor executor = new ConnectionExecutor(new HTTPConnectionHandler(handler), onConnect, onDisConnect);
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
            sListener = new TcpListener(ipEndPoint);
			ConfigureEvents();
            IsWork = true;
            thread = new Thread(ThreadFunc) { IsBackground = true, Priority = ThreadPriority.Highest };
            thread.Start();
		}

        public void Dispose()
        {
            if (!IsWork) { return; }
            IsWork = false;
            sListener.Stop();
            thread.Abort();
            thread.Join();
        }
    }
}
