using System;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using System.Security.Policy;
using System.Linq;
using System.Collections.Generic;
using HostInteractive;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommadInterfaces;
using CommadInterfaces.Exceptions;
using System.Threading;

namespace MainProgramm {
    class MainProgramm {
        private static Server MainServer;
        private static Dictionary<string, StringOIInterface> hosts;
        private static bool is_work = true;
        private static CommandArray cmds;

        private static void ConfigureCommands() {
            cmds = new CommandArray();
            cmds.AddCommand((c) =>
            {
                c.Name = () => "host";
                c.Active = true;
                c.MyData = () => null;
                c.ArgumentsCount = () => 1;
                c.Execute = (object[] arguments) => {
                    try {
                        string[] data = StringDataParser.Parse((string)arguments[0], 1);
                        StringOIInterface host = hosts[data[0]];
                        host.Write(data[1]);
                    }
                    catch (Exception err) {
                        Console.WriteLine("exception \r\n{0}", err);
                    }
                };
            });
            cmds.AddCommand((c) =>
            {
                c.Name = () => "lshost";
                c.Active = true;
                c.MyData = () => null;
                c.ArgumentsCount = () => 0;
                c.Execute = (object[] arguments) =>
                {
                    try
                    {
                        foreach (KeyValuePair<string, StringOIInterface> pair in hosts) { 
                            Console.WriteLine(pair.Key);
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("exception \r\n{0}", err);
                    }
                };
            });
            cmds.AddCommand((c) => {
                c.Name = () => "exit";
                c.Active = true;
                c.MyData = () => null;
                c.ArgumentsCount = () => 0;
                c.Execute = (object[] arguments) =>
                {
                    try
                    {
                        is_work = false;
                        Console.WriteLine("exit...");
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("exception \r\n{0}", err);
                    }
                };
            });
        }

        public static void Main(string[] args) {
            hosts = new Dictionary<string, StringOIInterface>();
            ConfigureCommands();
            XDocument config_doc = XDocument.Load(@"../../../config.xml");
            string doc_path = Path.GetFullPath(@"../../../config.xml");
            XElement[] host_configs = config_doc.Root.Elements().ToArray();
            int iterator = 0;
            object locker = new object();
            Func<int> GetIterator = new Func<int>(() => {
                lock (locker)
                {
                    int res = iterator;
                    iterator++;
                    return res;
                }
            });
            MainServer = new Server((obj) => 
            {
                StringOIInterface client = obj as StringOIInterface;
                int i = GetIterator();
                string name = host_configs[i].Element("name").Value;
                XElement message = new XElement("conf", new XElement("puth", doc_path), new XElement("name", name));
                client.Write(message.ToString());
                hosts.Add(name, client);
                Console.WriteLine(client.Read());
                client.Write("loadexplug");
                client.Write("loadintplug");
                client.Write("start");
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Console.WriteLine(client.Read());
                    }
                }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            });
            foreach (XElement host_conf in host_configs)
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
                resident.StartConnect(MainServer.Port);
            }

            while (is_work) {
                string cmd = Console.ReadLine();
                string[] cmd_name = StringDataParser.Parse(cmd, 1);
                try {
                    if (cmd_name.Length == 1) {
                        cmds.Execute(cmd_name[0]);
                    }
                    else { 
                        cmds.Execute(cmd_name[0], cmd_name[1]);
                    }
                }
                catch(CommandNotFound err) {
                    Console.WriteLine(err);
                }
                //StringOIInterface host = hosts[cmd_name[0]];
                //host.Write(res[1]);
            }

            //foreach (XElement host_conf in config_doc.Root.Elements())
            //{
            //    AppDomainSetup domaininfo = new AppDomainSetup();
            //    bool has_modules = true;
            //    try
            //    {
            //        domaininfo.ApplicationBase = host_conf.Element("modules_dir").Value;
            //    }
            //    catch (Exception err)
            //    {
            //        domaininfo.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //        has_modules = false;
            //    }
            //    Evidence adevidence = AppDomain.CurrentDomain.Evidence;
            //    AppDomain domain = AppDomain.CreateDomain(host_conf.Element("name").Value, adevidence, domaininfo);
            //    Resident resident = (Resident)domain.CreateInstanceAndUnwrap(
            //                typeof(Resident).Assembly.FullName,
            //                typeof(Resident).FullName);
            //    resident.AddConfig(host_conf.ToString());
            //    if (has_modules) resident.LoadPluginExternal();
            //    resident.LoadPluginInternal();
            //    resident.GetPluginInfo();
            //    resident.StartHost();
            //}
            Console.ReadLine();
        }
    }
}
