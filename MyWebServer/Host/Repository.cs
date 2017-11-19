using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Host.HttpHandler;
using Host.MIME;
using Config;

namespace Host {
	public static class Repository {
		public static Dictionary<string, IHttpHandler> ReqestsHandlers;
		public static Dictionary<string, IMIME> DataHandlers;
        public static WebServerConfig Configurate;
        public static IConfigRead ReadConfig {
            get { return Configurate; }
        }

        static Repository() {
            ReqestsHandlers = new Dictionary<string, IHttpHandler>();
            DataHandlers = new Dictionary<string, IMIME>();
        }
	}
}
