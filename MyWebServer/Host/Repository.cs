using System;
using System.Collections.Generic;
using Host.HttpHandler;
using Host.MIME;
using Config;
using Host.ServerExceptions;
using System.Xml.Linq;
using Host.DirReader;

namespace Host {
	public static class Repository {
		private static XElement _body_conf;
		public static XElement ConfigBody {
			get { return _body_conf; }
		}

		public static Dictionary<string, IHttpHandler> ReqestsHandlers;
		public static Dictionary<string, IMIME> DataHandlers;
		public static Dictionary<string, ExceptionFabric> ExceptionFabrics;
		public static IDirectoryReader DirReader;//экземпляр класса преобразующий директорию в html страницу
        public static WebServerConfig Configurate;

		public static int threads_count;

		static Repository() {
			ReqestsHandlers = new Dictionary<string, IHttpHandler>();
            DataHandlers = new Dictionary<string, IMIME>();
			ExceptionFabrics = new Dictionary<string, ExceptionFabric>();

			threads_count = 0;
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
