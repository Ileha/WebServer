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
using System.Text.RegularExpressions;

namespace MainProgramm {

    class MainProgramm {

        public static void Main(string[] args) {
            XDocument config_doc = XDocument.Load(@"../../../config.xml");
            string doc_path = Path.GetFullPath(@"../../../config.xml");
            int i = 0;
            foreach (XElement host_conf in config_doc.Root.Elements())
            {
                AppDomainSetup domaininfo = new AppDomainSetup();
				bool has_modules = true;
				try {
					domaininfo.ApplicationBase = host_conf.Element("modules_dir").Value;
				}
				catch (Exception err) {
					domaininfo.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					has_modules = false;
				}
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                AppDomain domain = AppDomain.CreateDomain(host_conf.Element("name").Value, adevidence, domaininfo);
                Resident resident = (Resident)domain.CreateInstanceAndUnwrap(
                            typeof(Resident).Assembly.FullName,
                            typeof(Resident).FullName);
                resident.AddConfig(doc_path, i);
				if (has_modules) resident.LoadPluginExternal();
                resident.LoadPluginInternal();
				resident.FileBrowser();
                resident.GetPluginInfo();
                resident.StartHost();
                i++;
            }
            Console.ReadLine();
        }
    }
}
