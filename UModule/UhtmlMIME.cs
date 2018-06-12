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

        public override void Handle(IConnetion Connection, out Action<Response, Reqest> Headers)
        {
            XDocument doc = XDocument.Load(Connection.ReadData.Data);
            string class_name = doc.Root.Element("header").Element("name").Value;
            Type NeedType = Type.GetType(class_name, true);
            ABSUModule page = (ABSUModule)Activator.CreateInstance(NeedType);
            page.Build(Connection, doc.Root.Element("body"));
            page.Init();
            page.Load();
            page.PreRender();
            page.Render();
            page.Unload();
            Headers = (response, reqest) =>
            {
                response.AddToHeader("Content-Type", page.ContentType, AddMode.rewrite);
            };
            page.Interact.Send();
        }
    }
}
