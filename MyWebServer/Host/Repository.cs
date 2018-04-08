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
		public static Dictionary<string, IHttpHandler> ReqestsHandlers;
		public static Dictionary<string, IMIME> DataHandlers;
		public static Dictionary<string, ExceptionFabric> ExceptionFabrics;
        public static WebServerConfig Configurate;
        public static List<IGrub> Eventers;

		static Repository() {
			ReqestsHandlers = new Dictionary<string, IHttpHandler>();
            DataHandlers = new Dictionary<string, IMIME>();
			ExceptionFabrics = new Dictionary<string, ExceptionFabric>();
            Eventers = new List<IGrub>();
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
