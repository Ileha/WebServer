using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Config
{
    public class ResoursePullPathToURL : ReactorPull {
		private Regex path;
		private string root_path;
		public ResoursePullPathToURL(string root) : base() {
			path = new Regex("/[^/]+");
			root_path = Path.GetFullPath(root);
		}
		public override ReactionValue Adder(XElement item)
		{
			return GetReaction(item.Value);
		}

		public override string GetDefaultValue(string path)
		{
			//path.Substring(
			Console.WriteLine(path);
			return path;
		}
		public override string OnCompaerReturn(ReactionValue RV, string get_path)
		{
			string s = path.Match(get_path).Value;
			Console.WriteLine(s);
			return Path.Combine(RV.ReturnValue, s);
		}
		public override bool Сomparer(string get_resourse, ReactionValue out_resourse)
		{
			return out_resourse.Reactor.IsMatch(get_resourse);
		}

		private ReactionValue GetReaction(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo inf = new DirectoryInfo(path);
                return new ReactionValue("^" + inf.FullName, inf.Name);
            }
            else if (File.Exists(path))
            {
                FileInfo inf = new FileInfo(path);
                return new ReactionValue("^" + inf.FullName + "$", "");
            }
            else {
                throw new FileNotFoundException("not found", path);
            }
        }
    }

    public class ResoursePullURLToPath : ReactorPull
    {
        private string _def;
        private Regex path;
        public ResoursePullURLToPath(string def_path) : base() {
            _def = def_path;
            path = new Regex(@"^/");
        }
        public override string GetDefaultValue(string url) {
			return Path.Combine(_def, path.Replace(url, ""));
        }
        public void AddReaction(string path)
        {
            _list_of_redirect.Add(GetReaction(path));
        }
        private ReactionValue GetReaction(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo inf = new DirectoryInfo(path);
                return new ReactionValue("^/" + inf.Name, inf.Parent.FullName);
            }
            else if (File.Exists(path))
            {
                FileInfo inf = new FileInfo(path);
                //string on_add = "";
                //try {
                //    on_add = item.Attribute("virtual_path").Value;
                //    on_add = path.Replace(on_add, "");
                //}
                //catch (Exception err) { }
                //Console.WriteLine(Path.Combine(on_add, inf.Name));
                return new ReactionValue("^/" + inf.Name + "$", inf.DirectoryName);
            }
            else
            {
                throw new FileNotFoundException("not found", path);
            }
        }
		public override string OnCompaerReturn(ReactionValue RV, string get_path)
		{
			return Path.Combine(RV.ReturnValue, path.Replace(get_path, ""));
		}
        public override ReactionValue Adder(XElement item)
        {
            return GetReaction(item.Value);
        }
        public override bool Сomparer(string get_resourse, ReactionValue out_resourse)
        {
            return out_resourse.Reactor.IsMatch(get_resourse);
        }
    }

    class ResourseManager
    {
    }
}
