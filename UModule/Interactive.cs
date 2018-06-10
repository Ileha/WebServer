using Host;
using Host.ConnectionHandlers;
using Host.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UModule
{
    public class Interactive
    {
        public Interactive(IConnetion data, XElement readData) {
            InputData = data.InputData;
            OutputData = data.OutputData;
            UserConnectData = data.UserConnectData;
            ConnectType = data.ConnectType;
            ReadData = new MemoryStream();
            readData.Save(ReadData);
            ReadData.Seek(0, SeekOrigin.Begin);
        }

        public Stream InputData
        {
            get;
            private set;
        }

        public Stream OutputData
        {
            get;
            private set;
        }

        public UserConnect UserConnectData
        {
            get;
            private set;
        }

        public MemoryStream ReadData
        {
            get;
            private set;
        }

        public ConnectionType ConnectType
        {
            get;
            private set;
        }
    }
}
