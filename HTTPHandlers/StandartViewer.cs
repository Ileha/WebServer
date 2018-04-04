using System;
using System.IO;
using Host.DirReader;
using Host;
using Resouces;
using Host.Eventer;

namespace DirViewer
{
	public class StandartViewer : IDirectoryReader, OnWebServerStart
	{
		public StandartViewer() {}
        public override string ItemPars(IItem file)
        {
            if (file.GetType() == typeof(LinkDirectory))
            {
                return "<p><a href=\"" + file.GetPath() + "\"><img src=\"WebServerResourses/folder.png\" height=\"20\"></img>" + file.GetName() + "</a>";
            }
            else if (file.GetType() == typeof(LinkFile)) {
                return "<p><a href=\""+file.GetPath()+"\"><img src=\"WebServerResourses/file.png\" height=\"20\"></img>" + file.GetName() + "</a>";
            }
            else { throw new InvalidDataException(); }
        }

		public override string UpFolder(IItem file) {
			return "<p><a href=\"" + file.GetPath() + "\"><img src=\"WebServerResourses/up.png\" height=\"20\"></img>..</a>";
		}

        public override string ThisFolder(IItem file) {
            return "<p><a href=\"" + file.GetPath() + "\"><img src=\"WebServerResourses/folder.png\" height=\"20\"></img>.</a>";
        }

		public void OnStart(EventArgs data) {
			Console.WriteLine("Test event start");
			Repository.Configurate.ResourceLinker.AddItem(new LinkDirectory(new DirectoryInfo(Repository.ConfigBody.Element("allow_browse_folders").Attribute("reourse_path").Value), Repository.Configurate.ResourceLinker, Repository.Configurate.Users.DefaultGroup));
		}
	}
}
