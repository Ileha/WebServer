using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using RequestHandlers;
using DataHandlers;
using ExceptionFabric;
using Events;
using HostInteractive;
using System.Text.RegularExpressions;
using CommadInterfaces;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace MainProgramm
{
    class CommandQueue : IDisposable
    {
        private ConcurrentQueue<Action> action_queue;
        private Task executor;
        private ManualResetEvent mre;
        private bool is_work;
        public CommandQueue()
        {
            action_queue = new ConcurrentQueue<Action>();
            mre = new ManualResetEvent(false);
            executor = Task.Factory.StartNew(task_funk, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            is_work = true;
        }

        private void task_funk()
        {
            while (true)
            {
                mre.WaitOne();
                if (!is_work) { break; }
                while (action_queue.Count != 0)
                {
                    Action act = null;
                    bool is_success = action_queue.TryDequeue(out act);
                    if (is_success)
                    {
                        act();
                    }
                }
                mre.Reset();
            }
        }

        public void AddTask(Action item)
        {
            action_queue.Enqueue(item);
            mre.Set();
        }

        public void Dispose()
        {
            is_work = false;
            mre.Set();
        }
    }

    public class Resident : MarshalByRefObject, IDisposable
    {
        private CommandArray array;
        private Client client;
        private CommandQueue CmdQueue;
        public void StartConnect(int port)
        {
            CmdQueue = new CommandQueue();
            client = new Client(port, (ioclient) =>
            {
                string puth_name = ioclient.Read();
                XElement message = XElement.Parse(puth_name);
                Repository.RepositoryConstruct(message.Element("puth").Value, message.Element("name").Value, ioclient.Write, ioclient.Write);
                ConfigureCommands();
                ioclient.Write(string.Format("Hello {0}", message.Element("name").Value));
                while (true)
                {
                    string cmd = ioclient.Read();
                    CmdQueue.AddTask(() =>
                    {
                        string[] body = StringDataParser.Parse(cmd, 1);
                        string[] args = null;
                        try
                        {
                            args = StringDataParser.Parse(body[1]);
                        }
                        catch (Exception err) { }
                        try
                        {
                            array.GetCommand(body[0]).OnExecute(args);
                        }
                        catch (Exception err)
                        {
                            ioclient.Write("exception \r\n{0}", err);
                        }
                    });
                }
            });
        }

        private void ConfigureCommands()
        {
            array = new CommandArray();
            array.AddCommand(c =>
            {
                c.Name = () => "loadintplug";//name of command
                c.ArgumentsCount = () => 0;//count of arguments in input
                c.MyData = () => null;//specific data of command
                c.Execute = (object[] arguments) =>
                {
                    try
                    {
                        LoadPluginInternal();
                    }
                    catch (Exception err)
                    {
                        Repository.Write("exception \r\n{0}", err);
                    }
                };//executable code
            });
            array.AddCommand(c =>
            {
                c.Name = () => "loadexplug";//name of command
                c.ArgumentsCount = () => 0;//count of arguments in input
                c.MyData = () => null;//specific data of command
                c.Execute = (object[] arguments) =>
                {
                    try
                    {
                        LoadPluginExternal();
                    }
                    catch (Exception err)
                    {
                        Repository.Write("exception \r\n{0}", err);
                    }
                };//executable code
            });
            array.AddCommand(c =>
            {
                c.Name = () => "info";//name of command
                c.ArgumentsCount = () => 0;//count of arguments in input
                c.MyData = () => null;//specific data of command
                c.Execute = (object[] arguments) =>
                {
                    try
                    {
                        Repository.Write(Repository.HostInfo());
                    }
                    catch (Exception err)
                    {
                        Repository.Write("exception \r\n{0}", err);
                    }
                };//executable code
            });
            array.AddCommand(c =>
            {
                c.Name = () => "start";//name of command
                c.ArgumentsCount = () => 0;//count of arguments in input
                c.MyData = () => null;//specific data of command
                c.Execute = (object[] arguments) =>
                {
                    try
                    {
                        Repository.Start();
                    }
                    catch (Exception err)
                    {
                        Repository.Write("exception \r\n{0}", err);
                    }
                };//executable code
            });
            array.AddCommand(c =>
            {
                c.Name = () => "stop";//name of command
                c.ArgumentsCount = () => 0;//count of arguments in input
                c.MyData = () => null;//specific data of command
                c.Execute = (object[] arguments) =>
                {
                    try
                    {
                        Repository.Stop();
                    }
                    catch (Exception err)
                    {
                        Repository.Write("exception \r\n{0}", err);
                    }
                };//executable code
            });
        }

        public void LoadPluginInternal()
        {
            Repository.WriteLine("loading internal plugins...");
            //загрузка обработчиков заголовков
            Type ourtype = typeof(ABSHttpHandler);
            IEnumerable<Type> list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                ABSHttpHandler h = (ABSHttpHandler)Activator.CreateInstance(t);
                try
                {
                    Repository.ReqestsHandlers.Add(h.IDHandler(), h);
                }
                catch (Exception err) { }
            }
            //загрузка обработчиков данных
            ourtype = typeof(ABSMIME);
            list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.GetInterfaces().Contains(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                ABSMIME h = (ABSMIME)Activator.CreateInstance(t);
                foreach (string extensions in h.file_extensions)
                {
                    try
                    {
                        Repository.DataHandlers.Add(extensions, h);
                    }
                    catch (Exception err) { }
                }
            }
            //загрузка фабрик исклчений
            ourtype = typeof(ABSExceptionFabric);
            list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                ABSExceptionFabric h = (ABSExceptionFabric)Activator.CreateInstance(t);
                try
                {
                    Repository.ExceptionFabrics.Add(h.name, h);
                }
                catch (Exception err) { }
            }
            //загрузка репозитроиев с событиями
            ourtype = typeof(ABSGrub);
            list = Assembly.GetAssembly(ourtype).GetTypes().Where(type => type.IsSubclassOf(ourtype) && type.IsClass);
            foreach (Type t in list)
            {
                ABSGrub h = (ABSGrub)Activator.CreateInstance(t);
                try
                {
                    Repository.Eventers.Add(h);
                }
                catch (Exception err) { }
            }
        }

        public void LoadPluginExternal()
        {
            Repository.WriteLine("loading external plugins...");
            FileInfo[] files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("*.dll");
            Type http_handler = typeof(ABSHttpHandler);
            Type mime_handler = typeof(ABSMIME);
            Type except_fabric = typeof(ABSExceptionFabric);
            Type eventers = typeof(ABSGrub);
            foreach (FileInfo fi in files)
            {
                Repository.WriteLine("loading {0}...", fi.FullName);
                Assembly load = Assembly.LoadFrom(fi.FullName);
                //загрузка обработчиков заголовков
                IEnumerable<Type> list = load.GetTypes().Where(type => type.IsSubclassOf(http_handler) && type.IsClass);
                foreach (Type t in list)
                {
                    ABSHttpHandler h = (ABSHttpHandler)Activator.CreateInstance(t);
                    try
                    {
                        Repository.ReqestsHandlers.Add(h.IDHandler(), h);
                    }
                    catch (Exception err) { }
                }
                //загрузка обработчиков данных
                list = load.GetTypes().Where(type => type.IsSubclassOf(mime_handler) && type.IsClass);
                foreach (Type t in list)
                {
                    ABSMIME h = (ABSMIME)Activator.CreateInstance(t);
                    foreach (string extensions in h.file_extensions)
                    {
                        try
                        {
                            Repository.DataHandlers.Add(extensions, h);
                        }
                        catch (Exception err) { }
                    }
                }
                //загрузка фабрик исклчений
                list = load.GetTypes().Where(type => type.IsSubclassOf(except_fabric) && type.IsClass);
                foreach (Type t in list)
                {
                    ABSExceptionFabric h = (ABSExceptionFabric)Activator.CreateInstance(t);
                    try
                    {
                        Repository.ExceptionFabrics.Add(h.name, h);
                    }
                    catch (Exception err) { }
                }
                //загрузка репозитроиев с событиями
                list = load.GetTypes().Where(type => type.IsSubclassOf(eventers) && type.IsClass);
                foreach (Type t in list)
                {
                    ABSGrub h = (ABSGrub)Activator.CreateInstance(t);
                    try
                    {
                        Repository.Eventers.Add(h);
                    }
                    catch (Exception err) { }
                }
                Repository.WriteLine("load");
            }
        }
        public void Info()
        {
            Repository.WriteLine("execute in domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }

        public void Dispose()
        {
            Repository.Configurate.Dispose();
        }
    }
}
