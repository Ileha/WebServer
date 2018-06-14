﻿using System;
using System.Net.Sockets;

namespace Host.ConnectionHandlers
{
	public interface IConnectionHandler : IDisposable {
        IConnetion GetConnetion { get; }
        IConnetion GetEventConnetion { get; }
        IConnectionHandler ExecuteHandler { get; }
		void Execute();
        void Reset();
	}
}
