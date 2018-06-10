using Host;
using Host.HttpHandler;
using Host.MIME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using Host.ServerExceptions;
using Host.Eventer;

namespace MainProgramm
{
    public class Resident : MarshalByRefObject, IDisposable
    {
        public void AddConfig(string data) {
			XElement doc = XElement.Parse(data);
			Repository.RepositoryConstruct(doc);
		}

        public void StartHost() {
			Repository.LoadWebServer();
			Repository.Configurate.Host.Start();
        }

        public void LoadPluginInternal() {
            Console.WriteLine("loading internal plugins...");
			//загрузка обработчиков заголовков
            Type ourtype = typeof(ABSHttpHandler);
			IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                ABSHttpHandler h = (ABSHttpHandler)Activator.CreateInstance(t);
                try
                {
					Repository.ReqestsHandlers.Add(h.IDHandler(), h);
                }
                catch (Exception err) {}
            }
			//загрузка обработчиков данных
            ourtype = typeof(ABSMIME);
            list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.GetInterfaces().Contains(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                ABSMIME h = (ABSMIME)Activator.CreateInstance(t);
				foreach (string extensions in h.file_extensions)
				{
					try
					{
						Repository.DataHandlers.Add(extensions, h);
					}
					catch (Exception err) { }
				}
            }
			//загрузка фабрик исклчений
			ourtype = typeof(ABSExceptionFabric);
            list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype) && type.IsClass);
			foreach (Type t in list) {
                ABSExceptionFabric h = (ABSExceptionFabric)Activator.CreateInstance(t);
                try {
					Repository.ExceptionFabrics.Add(h.name, h);
                }
                catch (Exception err) {}
            }
			//загрузка репозитроиев с функциями
            ourtype = typeof(ABSGrub);
            list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype) && type.IsClass);
            foreach (Type t in list) {
                ABSGrub h = (ABSGrub)Activator.CreateInstance(t);
                try {
                    Repository.Eventers.Add(h);
                }
                catch (Exception err) { }
            }
        }

        public void LoadPluginExternal()
        {
            Console.WriteLine("loading external plugins...");
            FileInfo[] files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll");
            Type http_handler = typeof(ABSHttpHandler);
            Type mime_handler = typeof(ABSMIME);
			Type except_fabric = typeof(ABSExceptionFabric);
            Type eventers = typeof(ABSGrub);
            foreach (FileInfo fi in files)
            {
                Console.WriteLine("loading {0}...", fi.FullName);
                Assembly load = Assembly.LoadFrom(fi.FullName);
				//загрузка обработчиков заголовков
				IEnumerable<Type> list = load.GetTypes().Where(type => type.IsSubclassOf(http_handler) && type.IsClass);
	            foreach (Type t in list) {
	                ABSHttpHandler h = (ABSHttpHandler)Activator.CreateInstance(t);
	                try {
						Repository.ReqestsHandlers.Add(h.IDHandler(), h);
	                }
	                catch (Exception err) {}
	            }
				//загрузка обработчиков данных
				list = load.GetTypes().Where(type => type.IsSubclassOf(mime_handler) && type.IsClass);
	            foreach (Type t in list) {
	                ABSMIME h = (ABSMIME)Activator.CreateInstance(t);
	                foreach (string extensions in h.file_extensions) {
						try
						{
							Repository.DataHandlers.Add(extensions, h);
						}
						catch (Exception err) { }
					}
	            }
				//загрузка фабрик исклчений
				list = load.GetTypes().Where(type => type.IsSubclassOf(except_fabric) && type.IsClass);
	            foreach (Type t in list) {
	                ABSExceptionFabric h = (ABSExceptionFabric)Activator.CreateInstance(t);
					try {
						Repository.ExceptionFabrics.Add(h.name, h);
					}
					catch (Exception err) { }
	            }
				//загрузка репозитроиев с функциями
				list = load.GetTypes().Where(type => type.IsSubclassOf(eventers) && type.IsClass);
                foreach (Type t in list) {
                    ABSGrub h = (ABSGrub)Activator.CreateInstance(t);
                    try {
                        Repository.Eventers.Add(h);
                    }
                    catch (Exception err) { }
                }
                Console.WriteLine("load");
            }
        }

        public void GetPluginInfo()
        {
            Console.WriteLine("\r\nHTTP Handlers:");
            foreach (KeyValuePair<string, ABSHttpHandler> handler in Repository.ReqestsHandlers)
            {
                Console.WriteLine("\tHTTP type {0}", handler.Key.ToString());
            }
            Console.WriteLine("MIME Handlers:");
            foreach (KeyValuePair<string, ABSMIME> handler in Repository.DataHandlers)
            {
                Console.WriteLine("\tMIME handler extension {0}", handler.Key);
            }
			Console.WriteLine("Available exceptions");
            foreach (KeyValuePair<string, ABSExceptionFabric> handler in Repository.ExceptionFabrics)
            {
                Console.WriteLine("\tException: {0}", handler.Key);
            }
        }
        public void Info()
        {
            Console.WriteLine("execute in domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }

        public void Dispose() {
            Repository.Configurate.Dispose();
        }
    }
}
