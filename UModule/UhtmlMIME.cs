using System;
using Host.MIME;
using Host;
using Host.ConnectionHandlers;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using UModule;

namespace HTTPHandlers
{
	public class UhtmlMIME : ABSMIME
	{
		private string[] _file_extensions = { ".uhtml" };
		public override string[] file_extensions { get { return _file_extensions; } }

		public override void Handle(IConnetion connection)
		{
			XDocument doc = XDocument.Load(connection.ReadData.data);
			string class_name = doc.Root.Element("header").Element("name").Value;
			Type NeedType = Type.GetType(class_name, true);
			ABSUModule page = (ABSUModule)Activator.CreateInstance(NeedType);
            page.Build(connection, doc.Element("body"));
            page.Init();
            page.Load();
            page.PreRender();
            page.Render();
            page.Unload();
		}

		public override void Headers(Response response, Reqest reqest, Reader read)
		{
			response.AddToHeader("Content-Type", "application/xhtml+xml", AddMode.rewrite);
		}
	}
}
