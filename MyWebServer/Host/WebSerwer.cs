using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using Config;

namespace Host {
    public delegate void HandlerExecutor();

    public class WebSerwer {
        private Socket sListener;
        private IPEndPoint ipEndPoint;
        private bool is_work;

        public WebSerwer() {
            IPAddress adres = IPAddress.Parse(Repository.ReadConfig["ip_adress"]);
            ipEndPoint = new IPEndPoint(adres, Convert.ToInt32(Repository.ReadConfig["port"]));
            sListener = new Socket(adres.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            is_work = true;
            Thread st = new Thread(this.ThreadFunc);
            st.Start();
        }

        private void ThreadFunc() {
            //DirectoryInfo dir = null;
            //try {
            //    dir = new DirectoryInfo(configuration["root_dir"]);
            //    configuration["root_dir"] = dir.FullName;
            //}
            //catch (ErrorServerConfig err) {
            //    return; //here may be cast to log file -> fatal error
            //}
            //try {
            //    string m_file = configuration["default_file"];
            //    if (!Regex.IsMatch(m_file, ".+\\.[^\\.]+")) {
            //        if (Regex.IsMatch(m_file, "\\.")) {
            //            m_file += "*";
            //        }
            //        else {
            //            m_file += ".*";
            //        }
            //    }

            //    try {
            //        FileInfo inf = dir.GetFiles(m_file, SearchOption.AllDirectories)[0];
            //        configuration["default_file"] = inf.FullName.Replace(configuration["root_dir"] + "/", "");
            //    }
            //    catch (Exception err) {
            //        configuration.Remove("default_file");
            //    }
            //}
            //catch (Exception err) {
            //    //here web server work like a file server
            //}

            //#if DEBUG
            //    try {
            //        Console.WriteLine(configuration["default_file"]);
            //    }
            //    catch (Exception err) { Console.WriteLine("this is file server"); }
            //#endif

            #if DEBUG
            Console.WriteLine("work {0}", Repository.ReadConfig["name"]);
            return;
            #endif

            try {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                Console.WriteLine("Запуск сервера {0}", Repository.ReadConfig["name"]);
                while (is_work) {
                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    #if DEBUG
                        Console.WriteLine("хост {1}, соединение через порт {0}", ipEndPoint, Repository.ReadConfig["name"]);
                    #endif

                    ConnectionHandler executor = new ConnectionHandler(handler);
                    HandlerExecutor execute = executor.Execute;
                    execute.BeginInvoke(null, null);
                }
            }
            catch (Exception err) {
                Console.WriteLine(err.ToString());
            }
        }
    }
}
