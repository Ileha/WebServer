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

namespace MainName {

	public class MyClass : MarshalByRefObject {
		public void GetAssemblyByName(string assemblyName) {
			Assembly.Load(assemblyName);
		}
		public void LoadPluginFrom() {
			FileInfo[] files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll");
			foreach (FileInfo fi in files)
			{
				Console.WriteLine("loading " + fi.FullName);
				// Получаем assemly из файла
				Assembly.LoadFrom(fi.FullName);
			    // Ищем нужный тип
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
			AppDomainSetup domaininfo = new AppDomainSetup();
			domaininfo.ApplicationBase = @"../../../HTTPHandlers/bin/Debug";
	        Evidence adevidence = AppDomain.CurrentDomain.Evidence;
			AppDomain domain = AppDomain.CreateDomain("dynamic domain", adevidence, domaininfo);

			MyClass resident = (MyClass)domain.CreateInstanceAndUnwrap(
						typeof(MyClass).Assembly.FullName,
						typeof(MyClass).FullName);
			

			//resident.Info();
			//resident.GetAssemblyByName("system, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			resident.LoadPluginFrom();
			//resident.GetAssembly(@"../../../add/bin/Debug/add.dll");

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
    }
}
