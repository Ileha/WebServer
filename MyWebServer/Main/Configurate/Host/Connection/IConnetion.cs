using System.IO;
using Configurate.Session;
using Configurate.Host.Connection.Reader;

namespace Configurate.Host.Connection
{
    public interface IConnetion {
        Stream InputData { get; }
        Stream OutputData { get; }
        UserConnect UserConnectData { get; }
        Reader.Reader ReadData { get; }
        string ConnectionType { get; }
    }
}
