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
//using UsI;

namespace MainName {

	public class MyClass : MarshalByRefObject {
		public Assembly GetAssemblyByName(string assemblyName) {
			try {
				return Assembly.Load(assemblyName);
			}
			catch (Exception) {
				return null;
			}
		}
		public void LoadPluginFrom() {
			FileInfo[] files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll");
			foreach (FileInfo fi in files)
			{
			    // Получаем assemly из файла
			    Assembly.LoadFile(fi.FullName);
			    // Ищем нужный тип
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
			

            InfoDomainApp(AppDomain.CurrentDomain);
			resident.Info();
			//resident.GetAssemblyByName("system, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			resident.LoadPluginFrom();
			//resident.GetAssembly(@"../../../add/bin/Debug/add.dll");
            InfoDomainApp(domain);

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

		static Assembly MyHandler(object source, ResolveEventArgs e)
		{
			Console.WriteLine("Resolving {0}", e.Name);
			Console.WriteLine("Resolve!!!");
			return Assembly.Load(e.Name);
		}

		private static IEnumerable<T> EnumerateExtensions<T>(AppDomain domain)
		{
			DirectoryInfo dir = new DirectoryInfo(domain.BaseDirectory);
			Console.WriteLine(dir.FullName);
			IEnumerable<FileInfo> fileNames = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
			if (fileNames != null)
			{
				foreach (FileInfo assemblyFileName in fileNames)
				{
					foreach (string typeName in GetTypes(assemblyFileName.FullName, typeof(T), domain))
					{
						System.Runtime.Remoting.ObjectHandle handle;
						try
						{
							handle = domain.CreateInstanceFrom(assemblyFileName.FullName, typeName);
						}
						catch (MissingMethodException)
						{
							continue;
						}
						object obj = handle.Unwrap();
						T extension = (T)obj;
						yield return extension;
					}
				}
			}
		}

		static AppDomain CreateDomain(string path) {
			AppDomainSetup setup = new AppDomainSetup();
			setup.ApplicationBase = new DirectoryInfo(path).FullName;
			return AppDomain.CreateDomain("Temporary domain", null, setup);
		}

		private static IEnumerable<string> GetTypes(string assemblyFileName, Type interfaceFilter, AppDomain domain) {
			Console.WriteLine("loading {0}", assemblyFileName);
			Assembly asm = domain.Load(AssemblyName.GetAssemblyName(assemblyFileName));
			Type[] types = asm.GetTypes();
			foreach (Type type in types)
			{
				//Console.WriteLine(type.ToString());
				if (type.GetInterface(interfaceFilter.Name) != null)
				{
					yield return type.FullName;
				}
			}
		}

		static void InfoDomainApp(AppDomain defaultD)
		{
			Console.WriteLine("*** Информация о домене приложения ***\n");
			Console.WriteLine("-> Имя: {0}\n-> ID: {1}\n-> По умолчанию? {2}\n-> Путь: {3}\n",
				defaultD.FriendlyName, defaultD.Id, defaultD.IsDefaultAppDomain(), defaultD.BaseDirectory);

			Console.WriteLine("Загружаемые сборки: \n");
			// Извлекаем информацию о загружаемых сборках с помощью LINQ-запроса
			var infAsm = defaultD.GetAssemblies();
			foreach (var a in infAsm) {
				//Console.WriteLine("-> Имя: \t{0}\n-> Версия: \t{1}", a.GetName().Name, a.GetName().Version);
				Console.WriteLine(a.FullName);
			}
		}
    }
}
