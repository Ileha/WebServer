using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Configurate.Host.Connection;

namespace UModule
{
    public abstract class ABSUModule
    {
        public Interactive Interact { get; private set; }
        public void Build(IConnetion Interact, out Stream data_stream, XElement data) {
            data_stream = new MemoryStream();
            this.Interact = new Interactive(Interact, data_stream, data);
        }
        public abstract void Handle(); 
        public abstract string ContentType { get; }
        protected void Write(string data) {
            byte[] _data = Encoding.UTF8.GetBytes(data);
            Interact.OutputData.Write(_data, 0, _data.Length);
        }
    }
}
