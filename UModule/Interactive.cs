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
using System.Text.RegularExpressions;

namespace UModule
{
    public class Interactive
    {
        public Interactive(IConnetion data, Stream writableStream, XElement readData) {
            InputData = data.InputData;
            UserConnectData = data.UserConnectData;
            ConnectType = data.ConnectType;
            ReadData = new MemoryStream();
            byte[] s_data = Encoding.UTF8.GetBytes(readData.Value);
            ReadData.Write(s_data, 0, s_data.Length);
            ReadData.Seek(0, SeekOrigin.Begin);
            OutputData = writableStream;
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
