using System;
using System.Net.Sockets;

namespace Host.ConnectionHandlers
{
	public interface IConnectionHandler {
        IConnetion GetConnetion { get; }
		IConnectionHandler ExecuteHandler();
		TcpClient Client { get; }
		void Clear();
	}
}
