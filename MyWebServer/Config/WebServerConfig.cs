using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using Resouces;

namespace Config
{
	public class WebServerConfig {
        private XElement _body_conf;
		public XElement ConfigBody { 
			get {
				return _body_conf;	
			} 
		}
        public readonly RedirectConfig RedirectConfigure;
        public IItem ResourceLinker;

        public WebServerConfig(XElement body) {
			_body_conf = body;
            ResourceLinker = new LinkDirectory(new DirectoryInfo(body.Element("root_dir").Value), null);
            RedirectConfigure = new RedirectConfig();
            //foreach (XElement el in body.Elements()) {
            //    if (el.Name.LocalName == "redirect_table") {
            //        RedirectConfigure.Configure(el);
            //    }
            //    if (el.Name.LocalName == "additive_dirs") {
            //        foreach (XElement add_dir in el.Elements())
            //        {
            //            if (Directory.Exists(add_dir.Value))
            //            {
            //                ResourceLinker.AddItem(new LinkDirectory(new DirectoryInfo(add_dir.Value), ResourceLinker));
            //            }
            //            else if (File.Exists(add_dir.Value))
            //            {
            //                ResourceLinker.AddItem(new LinkFile(new FileInfo(add_dir.Value), ResourceLinker));
            //            }
            //            else {}
            //        }
            //    }
            //    else if (!el.HasElements) {
            //        _body_conf.Add(el.Name.LocalName, el);
            //    }
            //}
        }
    }
}
