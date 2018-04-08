using System.IO;
using Host.Session;

namespace Host.ConnectionHandlers
{
    public enum ConnectionType {
        http,
        websocket
    }

    public interface IConnetion {
        Stream InputData { get; }
        Stream OutputData { get; }
        UserConnect UserConnectData { get; }
        Reader ReadData { get; }
        ConnectionType ConnectType { get; }
    }
}
