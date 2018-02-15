using System;
using System.Collections.Generic;
using System.IO;
using Config;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Resouces
{
    public class LinkDirectory : IItem, IConfigurate
    {
        public Dictionary<string, IItem> contain;
        public DirectoryInfo Resource;
        public IItem Parent;
        private DirectoryInfo directoryInfo;
        private FileSystemWatcher watcher;

        private string[] names = new string[] { "linker", "users" };
		public string[] ConfigName {
			get {
				return names;
			}
		}

		public LinkDirectory() : base() {}

        public LinkDirectory(DirectoryInfo inf, IItem parent, params string[] valid_groups)
            : base() {
			ConstructHelp(inf, parent, valid_groups);
        }

        private void ConstructHelp(DirectoryInfo inf, IItem parent, params string[] valid_groups)
        {
			contain = new Dictionary<string, IItem>();
            Resource = inf;
            Parent = parent;
            for (int i = 0; i < valid_groups.Length; i++) {
                Groups.Add(valid_groups[i]);
            }
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

        public override void AddItem(IItem adder_item)
        {
            contain.Add(adder_item.GetName(), adder_item);
        }

        public override string GetName()
        {
            return Resource.Name;
        }

        public override IItem GetParent()
        {
            return Parent;
        }

        public override FileSystemInfo GetInfo()
        {
            return Resource;
        }

        public override void Remove(IItem rem_item)
        {
            contain.Remove(rem_item.GetName());
        }

        public override IItem GetResourceByString(string path)
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

        public override IItem Element(string name)
        {
            return contain[name];
        }

        public override string GetPath()
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


        public override System.Collections.IEnumerator GetEnumerator()
        {
            return contain.Values.GetEnumerator();
        }


        public override void SetInfo(FileSystemInfo target, IItem New_parent)
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
            DirectoryInfo inf = new DirectoryInfo(data.Element("linker").Element("root_dir").Value);
            string[] def_groupes = Regex.Split(data.Element("users").Element("default_user").Attribute("groups").Value, ",");
            ConstructHelp(inf, null, def_groupes);
			try {
				XElement el = data.Element("linker").Element("additive_dirs");
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

            //foreach (XElement el in data.Element("linker").Element("resource_config").Elements()) {
            //    IItem resource = GetResourceByString(el.Value);
            //    string[] groups = Regex.Split(el.Attribute("groips").Value, ",");
            //    resource.AddGroupe(groups);
            //}
		}
	}
}
