using System;
using Host.MIME;
using Host;
using Host.ConnectionHandlers;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using UModule;
using Host.Session;

namespace HTTPHandlers
{
    public class UhtmlMIME : ABSMIME
    {
        private string[] _file_extensions = { ".uhtml" };
        public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, out Action<Response, Reqest> Headers)
        {
            ABSUModule page = null;
            XDocument doc = XDocument.Load(Connection.ReadData.Data);
            string class_name = doc.Root.Element("header").Element("name").Value;
            Type NeedType = Type.GetType(class_name, true);
            try {
                page = Connection.UserConnectData.GetData<ABSUModule>("data_handle");
                if (page.GetType() != NeedType) {
                    page = (ABSUModule)Activator.CreateInstance(NeedType);
                    Connection.UserConnectData.AddData("data_handle", page);
                }
            }
            catch (Exception err) {
                page = (ABSUModule)Activator.CreateInstance(NeedType);
                Connection.UserConnectData.AddData("data_handle", page);
            }
            
            Stream stream_with_data;
            page.Build(Connection, out stream_with_data, doc.Root.Element("body"));
            page.Handle(); 
            Headers = (response, reqest) =>
            {
                response.AddToHeader("Content-Type", page.ContentType, AddMode.rewrite);
            };
            stream_with_data.Seek(0, SeekOrigin.Begin);
            stream_with_data.CopyTo(Connection.OutputData);
        }
    }
}
