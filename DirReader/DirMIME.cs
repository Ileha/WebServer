using System;
using System.IO;
using System.Text;
using DataHandlers;
using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;
using Events;
using Configurate.Resouces;
using Configurate.Resouces.Items;

namespace Host.MIME
{
    public class DirMIME : ABSMIME, OnWebServerStart
    {
        private string[] _file_extensions = { ".dir" };
        public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, out Action<Response, Reqest> Headers)
        {
            Headers = (response, reqest) =>
            {
                response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
            };

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<!DOCTYPE html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n<title>{0}</title>\n</head>\n<body>\n", Connection.ReadData.Resourse.GetName());
            sb.AppendFormat("<p><a href=\"{0}\"><img src=\"WebServerResourses/folder.png\" height=\"20\"></img>.</a></p>\n", Connection.ReadData.Resourse.GetPath());
            if (Connection.ReadData.Resourse.Parent != null)
            {
                sb.AppendFormat("<p><a href=\"{0}\"><img src=\"WebServerResourses/up.png\" height=\"20\"></img>..</a></p>\n", Connection.ReadData.Resourse.Parent.GetPath());
            }
            foreach (IItem file in Connection.ReadData.Resourse)
            {
                if (file.Extension == ".dir")
                {
                    sb.AppendFormat("<p><a href=\"{0}\"><img src=\"WebServerResourses/folder.png\" height=\"20\"></img>{1}</a></p>\n", file.GetPath(), file.GetName());
                }
                else
                {
                    sb.AppendFormat("<p><a href=\"{0}\"><img src=\"WebServerResourses/file.png\" height=\"20\"></img>{1}</a></p>\n", file.GetPath(), file.GetName());
                }
            }

            sb.Append("\n</body>\n</html>");
            byte[] buff = Encoding.UTF8.GetBytes(sb.ToString());
            Connection.OutputData.Write(buff, 0, buff.Length);
            Connection.OutputData.Seek(0, SeekOrigin.Begin);
        }

        public void OnStart(HostEventData data)
        {
            Console.WriteLine("Test event start");
            Repository.Configurate.ResourceLinker.AddItem(new LinkDirectory(new DirectoryInfo("../../../Resourses/WebServerResourses"), Repository.Configurate.ResourceLinker, Repository.Configurate.Users.DefaultGroup));
        }
    }
}
