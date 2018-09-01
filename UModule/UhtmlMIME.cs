using System;
using DataHandlers;
using Configurate.Host.Connection;
using Configurate.Host.Connection.HTTPConnection;
using UModule;
using System.Xml.Linq;
using System.IO;

namespace HTTPHandlers
{
    public class UhtmlMIME : ABSMIME
    {
        private string[] _file_extensions = { ".uhtml" };
        public override string[] file_extensions { get { return _file_extensions; } }

        public override void Handle(IConnetion Connection, Action<string, string> add_to_http_header_request)
        {
            ABSUModule page = null;
            XDocument doc = XDocument.Load(Connection.ReadData.Data);
            string class_name = doc.Root.Element("header").Element("name").Value;
            Type NeedType = Type.GetType(class_name, true);
            bool is_data = false;
            try {
                page = Connection.UserConnectData.GetData<ABSUModule>("data_handle");
                if (page.GetType() != NeedType) {
                    page = (ABSUModule)Activator.CreateInstance(NeedType);
                    Connection.UserConnectData.AddData("data_handle", page);
                }
                else {
                    is_data = true;
                }
            }
            catch (Exception err) {
                page = (ABSUModule)Activator.CreateInstance(NeedType);
                Connection.UserConnectData.AddData("data_handle", page);
            }
            
            page.Build(Connection, doc.Root.Element("body"), (is_data && Connection.InputData.Length > 0));
            page.Handle();
            add_to_http_header_request("Content-Type", page.ContentType);
        }
    }
}
