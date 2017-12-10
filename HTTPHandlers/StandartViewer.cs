using System;
using System.IO;
using Host.DirReader;
using Host;

namespace DirViewer
{
	public class StandartViewer : IDirectoryReader, IHostEvents
	{
		public StandartViewer() {
			
		}

        public void OnStart() {
            Repository.Configurate._resourses.AddReaction(Repository.Configurate["allow_browse_folders"].Attribute("reourse_path").Value);
        }
        public void OnStop() {}
		public string DirPars(DirectoryInfo sub_dir) {
            return "<p><a href=\"WebServerResourses/folder.png\"><img src=\"WebServerResourses/folder.png\" height=\"20\"></img>"+sub_dir.Name+"</a>";
		}

		public string FilePars(FileInfo file) {
            return "<p><a href=\"WebServerResourses/file.png\"><img src=\"WebServerResourses/file.png\" height=\"20\"></img>" + file.Name + "</a>";
		}
	}
}
