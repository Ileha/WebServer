using System;
using System.IO;
using Host.DirReader;

namespace DirViewer
{
	public class StandartViewer : IDirectoryReader
	{
		public StandartViewer() {
			
		}

		public string DirPars(DirectoryInfo sub_dir) {
			throw new NotImplementedException();
		}

		public string FilePars(FileInfo file) {
			throw new NotImplementedException();
		}
	}
}
