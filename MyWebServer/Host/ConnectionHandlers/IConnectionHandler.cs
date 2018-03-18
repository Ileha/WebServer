using System;
using System.Net.Sockets;

namespace Host.ConnectionHandlers
{
	public interface IConnectionHandler {
		IConnectionHandler ExecuteHandler();
		TcpClient Client { get; }
		void Clear();
	}
}
