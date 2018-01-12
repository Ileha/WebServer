using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Host.HttpHandler;
using Host.MIME;
using Config;
using Host.DirReader;
using Host.Session;

namespace Host {
	public static class Repository {
		public static Dictionary<string, IHttpHandler> ReqestsHandlers;
		public static Dictionary<string, IMIME> DataHandlers;
        public static WebServerConfig Configurate;
		public static int threads_count;

        public static void RepositoryConstruct() {
            ReqestsHandlers = new Dictionary<string, IHttpHandler>();
            DataHandlers = new Dictionary<string, IMIME>();
			threads_count = 0;
        }
	}
}
