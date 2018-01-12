using System;
using System.Collections.Generic;
using System.IO;
using Config;
using System.Xml.Linq;

namespace Resouces
{
    public class LinkDirectory : IItem, IConfigurate
    {
        public Dictionary<string, IItem> contain;
        public DirectoryInfo Resource;
        public IItem Parent;
        private DirectoryInfo directoryInfo;
        private FileSystemWatcher watcher;

		public string ConfigName {
			get {
				return "linker";
			}
		}

		public LinkDirectory() {}

		public LinkDirectory(DirectoryInfo inf, IItem parent)
        {
			ConstructHelp(inf, parent);
        }

		private void ConstructHelp(DirectoryInfo inf, IItem parent) {
			contain = new Dictionary<string, IItem>();
            Resource = inf;
            Parent = parent;
            foreach (DirectoryInfo d in inf.GetDirectories()) {

				AddItem(new LinkDirectory(d, this));
            }
            foreach (FileInfo f in inf.GetFiles()) {

				AddItem(new LinkFile(f, this));
            }
            watcher = new FileSystemWatcher();
			watcher.Path = Resource.FullName;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += new FileSystemEventHandler(OnCreated);
			watcher.Deleted += new FileSystemEventHandler(OnDeleted);
			watcher.Renamed += new RenamedEventHandler(OnRenamed);
			watcher.EnableRaisingEvents = true;
		} 

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            try {
                if (IsFile(e.FullPath)) {
                    AddItem(new LinkFile(new FileInfo(e.FullPath), this));
                }
                else {
                    AddItem(new LinkDirectory(new DirectoryInfo(e.FullPath), this));
                }
            }
            catch (Exception err) {return;}
        }
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            contain.Remove(e.Name);
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            FileSystemInfo f = null;
            try {
                if (IsFile(e.FullPath)) {
                    f = new FileInfo(e.FullPath);
                }
                else {
                    f = new DirectoryInfo(e.FullPath);
                }
            }
            catch (Exception err) {return;}
            IItem t = contain[e.OldName];
            t.SetInfo(f, this);
            contain.Remove(e.OldName);
            AddItem(t);
        }

        private bool IsFile(string path) {
            if (File.Exists(path)) {
                return true;
            }
            else if (System.IO.Directory.Exists(path))
            {
                return false;
            }
            throw new FileNotFoundException(path);
        }

        public void AddItem(IItem adder_item)
        {
            contain.Add(adder_item.GetName(), adder_item);
        }

        public string GetName()
        {
            return Resource.Name;
        }

        public IItem GetParent()
        {
            return Parent;
        }

        public FileSystemInfo GetInfo()
        {
            return Resource;
        }

        public void Remove(IItem rem_item)
        {
            contain.Remove(rem_item.GetName());
        }

        public IItem GetResourceByString(string path)
        {
            string[] path_arr = path.Split('/');
            IItem result = this;
            foreach (string bit in path_arr) {
                try
                {
                    if (bit == ".") { }
                    else if (bit == "..")
                    {
                        result = Parent;
                    }
                    else if (bit != "")
                    {
                        result = result.Element(bit);
                    }
                }
                catch (Exception err)
                {
                    throw new FileNotFoundException(path_arr[path_arr.Length - 1]);
                }
            }
            return result;
        }

        public IItem Element(string name)
        {
            return contain[name];
        }

        public string GetPath()
        {
            IItem i = this;
            string res = "";

            while (i.GetParent() != null)
            {
                res = "/" + i.GetName() + res;
                i = i.GetParent();
            }
            return res;
        }


        public System.Collections.IEnumerator GetEnumerator()
        {
            return contain.Values.GetEnumerator();
        }


        public void SetInfo(FileSystemInfo target, IItem New_parent)
        {
            if (target is DirectoryInfo) {
                directoryInfo = target as DirectoryInfo;
                Parent = New_parent;
            }
            else {
                throw new FormatException(target.FullName);
            }
        }

		public void Configurate(XElement data) {
			DirectoryInfo inf = new DirectoryInfo(data.Element("root_dir").Value);
            ConstructHelp(inf, null);
			try {
				XElement el = data.Element("additive_dirs");
				foreach (XElement add_dir in el.Elements()) {
					if (Directory.Exists(add_dir.Value)) {
						AddItem(new LinkDirectory(new DirectoryInfo(add_dir.Value), this));
					}
					else if (File.Exists(add_dir.Value)) {
						AddItem(new LinkFile(new FileInfo(add_dir.Value), this));
					}
				}
			}
			catch (Exception err) {}
		}
	}
}
