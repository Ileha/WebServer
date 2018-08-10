using System.IO;
using Configurate.Session;
using Configurate.Host.Connection.Reader;

namespace Configurate.Host.Connection
{
    public enum ConnectionType {
        http,
        websocket
    }

    public interface IConnetion {
        Stream InputData { get; }
        Stream OutputData { get; }
        UserConnect UserConnectData { get; }
        Reader.Reader ReadData { get; }
        ConnectionType ConnectType { get; }
    }
}
