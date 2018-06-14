using System;
using System.Collections.Generic;
using System.IO;
using Config;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Host.Users;
using Host;

namespace Resouces
{
    public class LinkDirectory : IItem {
        public Dictionary<string, IItem> contain;
        public DirectoryInfo Resource;
        private DirectoryInfo directoryInfo;
        private FileSystemWatcher watcher;
		public override string Extension {
			get { return ".dir"; }
		}

		public LinkDirectory() : base() { }

        public LinkDirectory(DirectoryInfo inf, IItem parent, params GroupInfo[] valid_groups) : base() {
            ConstructHelp(inf, parent, valid_groups);
        }

        protected void ConstructHelp(DirectoryInfo inf, IItem parent, params GroupInfo[] valid_groups) {
            contain = new Dictionary<string, IItem>();
            Resource = inf;
            Parent = parent;
            for (int i = 0; i < valid_groups.Length; i++)
            {
                Groups.Add(valid_groups[i]);
            }
            foreach (DirectoryInfo d in inf.GetDirectories())
            {
                AddItem(new LinkDirectory(d, this, valid_groups));
            }
            foreach (FileInfo f in inf.GetFiles())
            {
                AddItem(new LinkFile(f, this, valid_groups));
            }
            watcher = new FileSystemWatcher();
            watcher.Path = Resource.FullName;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object source, FileSystemEventArgs e) {
            try {
                if (IsFile(e.FullPath)) {
                    AddItem(new LinkFile(new FileInfo(e.FullPath), this, Repository.Configurate.Users.DefaultGroup));
                }
                else {
                    AddItem(new LinkDirectory(new DirectoryInfo(e.FullPath), this, Repository.Configurate.Users.DefaultGroup));
                }
            }
            catch (Exception err) { return; }
        }
        private void OnDeleted(object source, FileSystemEventArgs e) {
            contain.Remove(e.Name);
        }
        private void OnRenamed(object source, RenamedEventArgs e) {
            FileSystemInfo f = null;
            try {
                if (IsFile(e.FullPath)) {
                    f = new FileInfo(e.FullPath);
                }
                else {
                    f = new DirectoryInfo(e.FullPath);
                }
            }
            catch (Exception err) { return; }
            IItem t = contain[e.OldName];
            t.SetInfo(f, this);
            contain.Remove(e.OldName);
            AddItem(t);
        }

        private bool IsFile(string path) {
            if (File.Exists(path)) {
                return true;
            }
            else if (System.IO.Directory.Exists(path)) {
                return false;
            }
            throw new FileNotFoundException(path);
        }

        public override void AddItem(IItem adder_item) {
            contain.Add(adder_item.GetName(), adder_item);
            adder_item.Parent = this;
        }

        public override string GetName() {
            return Resource.Name;
        }

        public override void Remove(IItem rem_item) {
            contain.Remove(rem_item.GetName());
            rem_item.Parent = null;
        }

        public override IItem GetResourceByString(string path) {
            string[] path_arr = path.Split('/');
            IItem result = this;
            foreach (string bit in path_arr) {
                try {
                    if (bit == ".") { }
                    else if (bit == "..") {
                        result = Parent;
                    }
                    else if (bit != "") {
                        result = result.Element(bit);
                    }
                }
                catch (Exception err) {
                    throw new FileNotFoundException(path_arr[path_arr.Length - 1]);
                }
            }
            return result;
        }

        public override IItem Element(string name) {
            return contain[name];
        }


        public override System.Collections.IEnumerator GetEnumerator() {
            return contain.Values.GetEnumerator();
        }


        public override void SetInfo(FileSystemInfo target, IItem New_parent) {
            if (target is DirectoryInfo) {
                directoryInfo = target as DirectoryInfo;
                Parent = New_parent;
            }
            else {
                throw new FormatException(target.FullName);
            }
        }

		public override void RemoveThis()
		{
			if (Parent != null)
			{
				Parent.Remove(this);
			}
			else {
				throw new Exception("unnable remove root");
			}
		}
	}
}
