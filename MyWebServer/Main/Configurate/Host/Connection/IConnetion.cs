using System.IO;
using Configurate.Session;
using Configurate.Host.Connection.Reader;
using System.Net.Sockets;
using System.Net;

namespace Configurate.Host.Connection
{
    public interface IConnetion {
        IPEndPoint RemoteEndPoint { get; }
        IPEndPoint LocalEndPoint { get; }
        Stream InputData { get; }
        Stream OutputData { get; }
        UserConnect UserConnectData { get; }
        Reader.Reader ReadData { get; }
        string ConnectionType { get; }
    }
}
