using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Configurate.Host.Connection;
using Configurate.Session;
using System.Net;

namespace UModule
{
    public class Interactive
    {
        public Interactive(IConnetion data, XElement readData) {
            InputData = data.InputData;
            UserConnectData = data.UserConnectData;
            ConnectionType = data.ConnectionType;
            ReadData = new MemoryStream();
            byte[] s_data = Encoding.UTF8.GetBytes(readData.Value);
            ReadData.Write(s_data, 0, s_data.Length);
            ReadData.Seek(0, SeekOrigin.Begin);
            OutputData = data.OutputData;
            RemoteEndPoint = data.RemoteEndPoint;
            LocalEndPoint = data.LocalEndPoint;
            URL = data.ReadData.URL;
        }
        public string URL { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }

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

        public string ConnectionType
        {
            get;
            private set;
        }
    }
}
