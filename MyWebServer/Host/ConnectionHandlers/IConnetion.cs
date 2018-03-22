using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Host.Session;

namespace Host.ConnectionHandlers
{
    public interface IConnetion {
        Stream InputData { get; }
        Stream OutputData { get; }
        UserConnect UserConnectData { get; }
        Reader ReadData { get; }
    }
}
