using System;
using System.Net.Sockets;

namespace Configurate.Host.Connection
{
	public interface IConnectionHandler : IDisposable {
        IConnetion GetConnetion { get; }
        IConnetion GetEventConnetion { get; }
        IConnectionHandler ExecuteHandler { get; }
		void Execute();
	}
}
