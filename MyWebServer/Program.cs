using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using MyWebServer.HttpHandler;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using MyWebServer.MIME;

namespace MyWebServer {
    class MainProgramm {
        public static Dictionary<TypeReqest, IHttpHandler> ReqestsHandlers;
        public static Dictionary<string, IMIME> DataHandlers;
        public static Dictionary<string, WebSerwer> hosts;

        public static void ConfigureHttpHandlers() {
            Type ourtype = typeof(IHttpHandler);
            ReqestsHandlers = new Dictionary<TypeReqest, IHttpHandler>();
            IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype));
            foreach (Type t in list) {
                IHttpHandler h = (IHttpHandler)Activator.CreateInstance(t);
                ReqestsHandlers.Add(h.HandlerType, h);
            }
        }

        public static void ConfigureDataHandlers() {
            Type ourtype = typeof(IMIME);
            DataHandlers = new Dictionary<string, IMIME>();
            IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.GetInterfaces().Contains(ourtype) && type.IsClass);
            foreach (Type t in list) {
                IMIME h = (IMIME)Activator.CreateInstance(t);
                DataHandlers.Add(h.file_extension, h);
            }
        }

        public static void Main(string[] args) {
            ConfigureHttpHandlers();
            ConfigureDataHandlers();

            XDocument doc = XDocument.Load("../../../config.xml");
            int i = 0;
            foreach (XElement el in doc.Root.Elements()) {
                string name = "";
                try {
                    name = el.Element("name").Value;
                }
                catch (Exception err) {
                    name = "server_" + i;
                }
                string target_name = "";
                try {
                    target_name = el.Element("default_file").Value;
                }
                catch (Exception err) {
                    target_name = null;
                }
                new WebSerwer(el.Element("ip_adress").Value,
                              Convert.ToInt32(el.Element("port").Value),
                              el.Element("root_dir").Value,
                              name,
                              target_name);
                i++;
            }

            //IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            //IPAddress ipAddr = ipHost.AddressList[0];
            //new WebSerwer(ipAddr, 11000, "/Users/Alexey/Documents/Programm Projects/C#/WebServer/Resourses");
            Console.ReadLine();
        }
    }
}
