using Configurate.Host.Connection;
using Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UModule
{
    public class Loader : ABSGrub
    {
        public override void OnStart(HostEventData data)
        {
            Console.WriteLine("start");
        }
        public override void OnStop(HostEventData data)
        {
            Console.WriteLine("stop");
        }
        public override void OnConntect(ConnectionEventData data) 
        {
            Console.WriteLine("Connect {0}", data.connect.ConnectType);
            if (data.connect.ConnectType == ConnectionType.websocket) {
                byte[] _data = Encoding.UTF8.GetBytes("you connect");
                data.connect.OutputData.Write(_data, 0, _data.Length);
            }
        }
        public override void OnDisConntect(ConnectionEventData data)
        {
            Console.WriteLine("disconnect {0}", data.connect.ConnectType);
        }
    }
}
