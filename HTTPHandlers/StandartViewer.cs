using System;
using System.IO;
using Host.DirReader;
using Host;
using Resouces;
using System.Xml.Linq;

namespace DirViewer
{
	public class StandartViewer : IDirectoryReader, IHostEvents
	{
		public string ConfigName {
			get {
				return "allow_browse_folders";
			}
		}

		public StandartViewer() {
			
		}

		public void OnStart() {
			Console.WriteLine("Test event start");
		}
        public void OnStop() {}
        public string ItemPars(IItem file)
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

		public void Configurate(XElement data)
		{
			Repository.Configurate.ResourceLinker.AddItem(new LinkDirectory(new DirectoryInfo(data.Attribute("reourse_path").Value), Repository.Configurate.ResourceLinker));
		}
	}
}
