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

namespace MainProgramm {

    class MainProgramm {
		//public static Dictionary<string, WebSerwer> hosts;

        public static void Main(string[] args) {
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

            XDocument config_doc = XDocument.Load(@"../../../config.xml");
            int i = 0;
            foreach (XElement host_conf in config_doc.Root.Elements()) {
                AppDomainSetup domaininfo = new AppDomainSetup();
                domaininfo.ApplicationBase = @"../../../HTTPHandlers/bin/Debug";
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                AppDomain domain = AppDomain.CreateDomain("test_domain", adevidence, domaininfo);
                Resident resident = (Resident)domain.CreateInstanceAndUnwrap(
                            typeof(Resident).Assembly.FullName,
                            typeof(Resident).FullName);
                resident.LoadPluginFrom();
                resident.GetPluginInfo();
            }
            Console.ReadLine();
        }
    }
}
