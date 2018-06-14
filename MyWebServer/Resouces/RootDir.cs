using Config;
using Host;
using Host.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Resouces
{
    public class RootDir : LinkDirectory, IConfigurate {

        public RootDir() : base() { }

        private string[] names = new string[] { "linker", "users" };
        public string[] ConfigName { get { return names; } }

        public void Configurate(XElement data)
        {
            DirectoryInfo inf = new DirectoryInfo(data.Element("linker").Element("root_dir").Value);
            ConstructHelp(inf, null, Repository.Configurate.Users.DefaultGroup);

            try
            {
                XElement el = data.Element("linker").Element("additive_dirs");
                foreach (XElement add_dir in el.Elements())
                {
                    if (Directory.Exists(add_dir.Value))
                    {
                        AddItem(new LinkDirectory(new DirectoryInfo(add_dir.Value), this, Repository.Configurate.Users.DefaultGroup));
                    }
                    else if (File.Exists(add_dir.Value))
                    {
                        AddItem(new LinkFile(new FileInfo(add_dir.Value), this, Repository.Configurate.Users.DefaultGroup));
                    }
                }
            }
            catch (Exception err) { }
            try
            {
                XElement el = data.Element("linker").Element("remove_dirs");
                foreach (XElement rm_dir in el.Elements())
                {
                    try
                    {
                        GetResourceByString(rm_dir.Value).RemoveThis();
                    }
                    catch (Exception err) { }
                }
            }
            catch (Exception err) { }

            foreach (XElement el in data.Element("linker").Element("resource_config").Elements())
            {
                IItem resource;
                try
                {
                    resource = GetResourceByString(el.Value);
                }
                catch (FileNotFoundException err) { continue; }
                string[] groups = Regex.Split(el.Attribute("groups").Value, ",");
                resource.ClearAllGroupe();
                for (int i = 0; i < groups.Length; i++)
                {
                    GroupInfo gr;
                    try
                    {
                        gr = Repository.Configurate.Users.groups[groups[i]];
                    }
                    catch (Exception err) { continue; }
                    resource.AddGroupe(gr);
                }
            }
        }
    }
}
