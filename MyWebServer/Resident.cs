using Config;
using Host;
using Host.HttpHandler;
using Host.MIME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Host;

namespace MainProgramm
{
    public class Resident : MarshalByRefObject
    {
        public void AddConfig(WebServerConfig config) {
            Repository.Configurate = config;
        }
        public void StartHost() {
            new Host.WebSerwer();
        }

        public void LoadPluginInternal() {
            Console.WriteLine("loading internal plugins...");
            Type ourtype = typeof(IHttpHandler);
            IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.GetInterfaces().Contains(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                IHttpHandler h = (IHttpHandler)Activator.CreateInstance(t);
                try
                {
                    Repository.ReqestsHandlers.Add(h.HandlerType, h);
                }
                catch (Exception err) {}
            }
            ourtype = typeof(IMIME);
            list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.GetInterfaces().Contains(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                IMIME h = (IMIME)Activator.CreateInstance(t);
                try
                {
                    Repository.DataHandlers.Add(h.MIME_Type, h);
                }
                catch (Exception err) { }
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
                foreach (Type t in load.GetTypes())
                {
                    Type[] interfaces = t.GetInterfaces();
                    if (interfaces.Contains(http_handler))
                    {
                        IHttpHandler https = Activator.CreateInstance(t) as IHttpHandler;
                        Repository.ReqestsHandlers.Add(https.HandlerType, https);
                    }
                    else if (interfaces.Contains(mime_handler))
                    {
                        IMIME mime = Activator.CreateInstance(t) as IMIME;
                        Repository.DataHandlers.Add(mime.file_extension, mime);
                    }
                }
                Console.WriteLine("load");
            }
        }

        public void GetPluginInfo()
        {
            Console.WriteLine("HTTP Handlers:");
            foreach (KeyValuePair<TypeReqest, IHttpHandler> handler in Repository.ReqestsHandlers)
            {
                Console.WriteLine("HTTP type {0}", handler.Key.ToString());
            }
            Console.WriteLine("MIME Handlers:");
            foreach (KeyValuePair<string, IMIME> handler in Repository.DataHandlers)
            {
                Console.WriteLine("MIME handler extension {0}", handler.Key);
            }
        }
        public void Info()
        {
            Console.WriteLine("execute in domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
