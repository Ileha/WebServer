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
            Console.WriteLine("Start!!!");
        }
        public void OnStop() {
            Console.WriteLine("Stop!!!");
        }
		public string DirPars(DirectoryInfo sub_dir) {
			throw new NotImplementedException();
		}

		public string FilePars(FileInfo file) {
			throw new NotImplementedException();
		}
	}
}
