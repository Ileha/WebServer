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
        Stream OutputData { get; set; }
        UserConnect UserConnectData { get; }
        Reader ReadData { get; }
        ConnectionType ConnectType { get; }
    }
}
