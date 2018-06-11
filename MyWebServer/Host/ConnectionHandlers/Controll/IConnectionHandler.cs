using System;
using System.Net.Sockets;

namespace Host.ConnectionHandlers
{
	public interface IConnectionHandler : IDisposable {
        IConnetion GetConnetion { get; }
        IConnectionHandler ExecuteHandler { get; }
		void Execute();
        void Reset();
	}
}
