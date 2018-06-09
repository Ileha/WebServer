using System;
using System.Collections.Generic;
using Host.HttpHandler;
using Host.MIME;
using Config;
using Host.ServerExceptions;
using System.Xml.Linq;
using Host.Eventer;

namespace Host {
	public static class Repository {
		private static XElement _body_conf;
		public static XElement ConfigBody {
			get { return _body_conf; }
		}
		public static Dictionary<string, ABSHttpHandler> ReqestsHandlers;
		public static Dictionary<string, ABSMIME> DataHandlers;
		public static Dictionary<string, ABSExceptionFabric> ExceptionFabrics;
        public static WebServerConfig Configurate;
        public static List<ABSGrub> Eventers;

		static Repository() {
			ReqestsHandlers = new Dictionary<string, ABSHttpHandler>();
            DataHandlers = new Dictionary<string, ABSMIME>();
			ExceptionFabrics = new Dictionary<string, ABSExceptionFabric>();
            Eventers = new List<ABSGrub>();
		}

        public static void RepositoryConstruct(XElement doc) {
			_body_conf = doc;
        }

		public static void LoadWebServer() {
			Configurate = new WebServerConfig();
            Configurate.Configurate();
		}
	}
}
