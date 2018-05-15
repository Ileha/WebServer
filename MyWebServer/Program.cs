using System;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using System.Security.Policy;
using System.Linq;
using System.Collections.Generic;

namespace MainProgramm {

    class MainProgramm {
        public static void Main(string[] args) {
            XDocument config_doc = XDocument.Load(@"../../../config.xml");
            string doc_path = Path.GetFullPath(@"../../../config.xml");
            List<Resident> Controls = new List<Resident>();
            foreach (XElement host_conf in config_doc.Root.Elements())
            {
                AppDomainSetup domaininfo = new AppDomainSetup();
                bool has_modules = true;
                try
                {
                    domaininfo.ApplicationBase = host_conf.Element("modules_dir").Value;
                }
                catch (Exception err)
                {
                    domaininfo.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    has_modules = false;
                }
                Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                AppDomain domain = AppDomain.CreateDomain(host_conf.Element("name").Value, adevidence, domaininfo);
                Resident resident = (Resident)domain.CreateInstanceAndUnwrap(
                            typeof(Resident).Assembly.FullName,
                            typeof(Resident).FullName);
                resident.AddConfig(host_conf.ToString());
				if (has_modules) resident.LoadPluginExternal();
                resident.LoadPluginInternal();
                resident.GetPluginInfo();
                resident.StartHost();
                Controls.Add(resident);
            }
            Console.ReadLine();
            foreach (Resident host_controll in Controls)
            {
                host_controll.Dispose();
            }
        }
    }
}
