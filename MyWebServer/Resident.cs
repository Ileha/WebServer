using Config;
using Host;
using Host.HttpHandler;
using Host.MIME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using Host.DirReader;
using System.Text.RegularExpressions;

namespace MainProgramm
{
	public delegate void OnResidentLoadEvent();
	public delegate void ForAddEvent(OnResidentLoadEvent new_event);

    public class Resident : MarshalByRefObject
    {
		private event OnResidentLoadEvent OnEndLoad;

        public void AddConfig(string data) {
			XElement doc = XElement.Parse(data);
			Repository.RepositoryConstruct();
            Repository.Configurate = new WebServerConfig(doc, AddToLoad);
		}

		private void AddToLoad(OnResidentLoadEvent new_event) {
			OnEndLoad += new_event;
		}
        public void StartHost() {
			if (OnEndLoad != null) {
				OnEndLoad();
			}
			Repository.Configurate.Host.Start();
        }

		public void FileBrowser() {
			string name;
			try {
				if (Repository.Configurate["allow_browse_folders"].Attribute("is_work").Value != "true") { return; }
				name = Repository.Configurate["allow_browse_folders"].Attribute("browser").Value;
			}
			catch (Exception err) {
				return;
			}
			AppDomain currentDomain = AppDomain.CurrentDomain;
			Assembly[] assems = currentDomain.GetAssemblies();
			Console.WriteLine("loading Browser");
			Type ourtype = typeof(IDirectoryReader);
			foreach (Assembly assem in assems) {
				//Console.WriteLine("find assembly: {0}", assem.ToString());
				IEnumerable<Type> list = assem.GetTypes().Where((arg) => arg.GetInterfaces().Contains(ourtype) && arg.IsClass);
				foreach (Type t in list) {
					if (name == t.Name) {
						Repository.Configurate.DirReader = (IDirectoryReader)Activator.CreateInstance(t);
						break;
					}
				}
			}
		}

        public void LoadPluginInternal() {
            Console.WriteLine("loading internal plugins...");
            Type ourtype = typeof(IHttpHandler);
			IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                IHttpHandler h = (IHttpHandler)Activator.CreateInstance(t);
                try
                {
					Repository.ReqestsHandlers.Add(h.IDHandler(), h);
                }
                catch (Exception err) {}
            }
            ourtype = typeof(IMIME);
            list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.GetInterfaces().Contains(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                IMIME h = (IMIME)Activator.CreateInstance(t);
				foreach (string extensions in h.file_extensions)
				{
					try
					{
						Repository.DataHandlers.Add(extensions, h);
					}
					catch (Exception err) { }
				}
            }
        }

        public void LoadPluginExternal()
        {
            Console.WriteLine("loading external plugins...");
            FileInfo[] files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll");
            Type http_handler = typeof(IHttpHandler);
            Type mime_handler = typeof(IMIME);
            foreach (FileInfo fi in files)
            {
                Console.WriteLine("loading {0}...", fi.FullName);
                Assembly load = Assembly.LoadFrom(fi.FullName);
				IEnumerable<Type> list = load.GetTypes().Where(type => type.IsSubclassOf(http_handler) && type.IsClass);
	            foreach (Type t in list) {
	                IHttpHandler h = (IHttpHandler)Activator.CreateInstance(t);
	                try {
						Repository.ReqestsHandlers.Add(h.IDHandler(), h);
	                }
	                catch (Exception err) {}
	            }
				list = load.GetTypes().Where(type => type.GetInterfaces().Contains(mime_handler) && type.IsClass);
	            foreach (Type t in list) {
	                IMIME h = (IMIME)Activator.CreateInstance(t);
	                foreach (string extensions in h.file_extensions) {
						try
						{
							Repository.DataHandlers.Add(extensions, h);
						}
						catch (Exception err) { }
					}
	            }
                Console.WriteLine("load");
            }
        }

        public void GetPluginInfo()
        {
            Console.WriteLine("HTTP Handlers:");
            foreach (KeyValuePair<string, IHttpHandler> handler in Repository.ReqestsHandlers)
            {
                Console.WriteLine("HTTP type {0}", handler.Key.ToString());
            }
            Console.WriteLine("MIME Handlers:");
            foreach (KeyValuePair<string, IMIME> handler in Repository.DataHandlers)
            {
                Console.WriteLine("MIME handler extension {0}", handler.Key);
            }

			Console.WriteLine("Browser module: {0}", Repository.Configurate.DirReader == null ? "" : Repository.Configurate.DirReader.GetType().ToString());
        }
        public void Info()
        {
            Console.WriteLine("execute in domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
