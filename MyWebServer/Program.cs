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
            XDocument config_doc = XDocument.Load(@"../../../config.xml");
            string doc_path = Path.GetFullPath(@"../../../config.xml");
            int i = 0;
            foreach (XElement host_conf in config_doc.Root.Elements()) {
                AppDomainSetup domaininfo = new AppDomainSetup();
                domaininfo.ApplicationBase = @"../../../HTTPHandlers/bin/Debug";
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                AppDomain domain = AppDomain.CreateDomain("test_domain", adevidence, domaininfo);
                Resident resident = (Resident)domain.CreateInstanceAndUnwrap(
                            typeof(Resident).Assembly.FullName,
                            typeof(Resident).FullName);
                resident.AddConfig(doc_path, i);
                resident.LoadPluginExternal();
                resident.LoadPluginInternal();
                resident.GetPluginInfo();
                resident.StartHost();
                i++;
            }
            Console.ReadLine();
        }
    }
}
