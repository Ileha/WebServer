using Resouces;
using Config;
using System.Text;

namespace Host.DirReader
{
    public abstract class IDirectoryReader
    {
		public virtual string BeforeData(IItem root) {
			return "<!DOCTYPE html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n<title>"+root.GetName()+"</title>\n</head>\n<body>\n";
		}
		public virtual string UpFolder(IItem file) { return ""; }
		public virtual string ThisFolder(IItem file) { return ""; }
        public abstract string ItemPars(IItem file);
		public virtual string AfterData() {
			return "\n</body>\n</html>";
		}

		public string ParsDirectoryHeader(IItem dir) {
			string res = "";
			res += BeforeData(dir);
			res += ThisFolder(dir);
			if (dir.Parent != null) {
				res += UpFolder(dir.Parent);
			}

			return res;
		}
		public string ParsDirectoryDown(IItem dir) {
			return AfterData();
		}
    }
}
