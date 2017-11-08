using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Host.HttpHandler;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using Host.MIME;
using Config;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Remoting;
using System.Security.Policy;
using Host;

namespace MainName {

	public class Resident : MarshalByRefObject {
		public void LoadPluginFrom() {
			FileInfo[] files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll");
			Type http_handler = typeof(IHttpHandler);
            Type mime_handler = typeof(IMIME);
            foreach (FileInfo fi in files) {
				Console.WriteLine("loading {0}...", fi.FullName);
				Assembly load = Assembly.LoadFrom(fi.FullName);
                foreach (Type t in load.GetTypes()) {
                    Type[] interfaces = t.GetInterfaces();
                    if (interfaces.Contains(http_handler)) {
                        IHttpHandler https = Activator.CreateInstance(t) as IHttpHandler;
                        Repository.ReqestsHandlers.Add(https.HandlerType, https);
                    }
                    else if (interfaces.Contains(mime_handler)) {
                        IMIME mime = Activator.CreateInstance(t) as IMIME;
                        Repository.DataHandlers.Add(mime.file_extension, mime);
                    }
                }
				Console.WriteLine("load");
			}
		}
		public void Info() {
			Console.WriteLine("execute in domain {0}", AppDomain.CurrentDomain.FriendlyName);
		}
	}

    class MainProgramm {
		//public static Dictionary<string, WebSerwer> hosts;

        public static void Main(string[] args) {
			//DirectoryInfo dir = new DirectoryInfo(domain.BaseDirectory);
			//IEnumerable<FileInfo> fileNames = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
			//if (fileNames != null)
			//{
			//	foreach (FileInfo assemblyFileName in fileNames)
			//	{
			//		class_inst.Info();
			//	}
			//}
            //foreach (XElement el in doc.Root.Elements()) {
                //new WebSerwer(new WebServerConfig(el));
                //string name = "";
                //try {
                //    name = el.Element("name").Value;
                //}
                //catch (Exception err) {
                //    name = "server_" + i;
                //}
                //string target_name = "";
                //try {
                //    target_name = el.Element("default_file").Value;
                //}
                //catch (Exception err) {
                //    target_name = null;
                //}
                //new WebSerwer(el.Element("ip_adress").Value,
                //              Convert.ToInt32(el.Element("port").Value),
                //              el.Element("root_dir").Value,
                //              name,
                //              target_name);
                //i++;
            //}
            //IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            //IPAddress ipAddr = ipHost.AddressList[0];
            //new WebSerwer(ipAddr, 11000, "/Users/Alexey/Documents/Programm Projects/C#/WebServer/Resourses");
            Console.ReadLine();
        }

        public Resident CreateDomain(string _domain_name, string _base_directory) {
            AppDomainSetup domaininfo = new AppDomainSetup();
            domaininfo.ApplicationBase = _base_directory;//@"../../../HTTPHandlers/bin/Debug";
            Evidence adevidence = AppDomain.CurrentDomain.Evidence;
            AppDomain domain = AppDomain.CreateDomain(_domain_name, adevidence, domaininfo);

            Resident resident = (Resident)domain.CreateInstanceAndUnwrap(
                        typeof(Resident).Assembly.FullName,
                        typeof(Resident).FullName);
            resident.LoadPluginFrom();
            return resident;
        } 
    }
}
