using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace MyWebServer {
    public delegate void HandlerExecutor();

    public class WebSerwer {
        private Socket sListener;
        private IPEndPoint ipEndPoint;
        private bool is_work;
        private Func<string> _workDirectory;
        public Func<string> workDirectory { get { return _workDirectory; } }
        public readonly string Name;
        private Func<string> _getMainfile;
        public Func<string> getMainfile { 
            get { 
                return _getMainfile; 
            } 
        }
        public readonly bool if_file_server;

        public WebSerwer(IPAddress adres, int port, string work_dir, string name, string main_file) {
            if (main_file != null) {
                _getMainfile = () => main_file;
                if_file_server = false;
            }
            else {
                _getMainfile = () => null;
                if_file_server = true;
            }
            Name = name;
            _workDirectory = () => work_dir;
            ipEndPoint = new IPEndPoint(adres, port);
            sListener = new Socket(adres.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            is_work = true;
            Thread st = new Thread(this.ThreadFunc);
            st.Start();
        }

        public WebSerwer(string adres, int port, string work_dir, string name, string main_file) : this(IPAddress.Parse(adres), 
                                                                                                       port, work_dir, name, main_file) {
            
        }

        private void ThreadFunc() {
            string m_file = getMainfile();
            if (!Regex.IsMatch(m_file, "[^\\.]\\.[^\\.]")) {
                if (Regex.IsMatch(m_file, "\\.")) { 
                    m_file += "*"; 
                }
                else {
                    m_file += ".*";
                }
            }
            DirectoryInfo dir = new DirectoryInfo(workDirectory());
            _workDirectory = () => dir.FullName;
            FileInfo inf = dir.GetFiles(m_file, SearchOption.AllDirectories)[0];
            m_file = inf.FullName.Replace(workDirectory(), "");
            _getMainfile = () => m_file;
            Console.WriteLine(getMainfile());

            try {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                Console.WriteLine("Запуск сервера");
                while (is_work) {
                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    Console.WriteLine("соединение через порт {0}", ipEndPoint);

                    ConnectionHandler executor = new ConnectionHandler(handler, this);
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
