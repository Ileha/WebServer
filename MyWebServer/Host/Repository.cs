using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Host.HttpHandler;
using Host.MIME;
using Config;

namespace Host {
	public static class Repository {
		public static Dictionary<TypeReqest, IHttpHandler> ReqestsHandlers;
		public static Dictionary<string, IMIME> DataHandlers;
        public static WebServerConfig Configurate;
        public static IConfigRead ReadConfig {
            get { return Configurate; }
        }

        static Repository() {
            ReqestsHandlers = new Dictionary<TypeReqest, IHttpHandler>();
            DataHandlers = new Dictionary<string, IMIME>();
        }

        //public static void ConfigureHttpHandlers() {
        //    Type ourtype = typeof(IHttpHandler);
        //    ReqestsHandlers = new Dictionary<TypeReqest, IHttpHandler>();
        //    IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype));
        //    foreach (Type t in list) {
        //        IHttpHandler h = (IHttpHandler)Activator.CreateInstance(t);
        //        ReqestsHandlers.Add(h.HandlerType, h);
        //    }
        //}

        //public static void ConfigureDataHandlers()
        //{
        //    Type ourtype = typeof(IMIME);
        //    DataHandlers = new Dictionary<string, IMIME>();
        //    IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.GetInterfaces().Contains(ourtype) && type.IsClass);
        //    foreach (Type t in list)
        //    {
        //        IMIME h = (IMIME)Activator.CreateInstance(t);
        //        DataHandlers.Add(h.file_extension, h);
        //    } 
        //}

	}
}
