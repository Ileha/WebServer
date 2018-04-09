﻿using System;
using Host.Eventer;

namespace Host.ConnectionHandlers
{
	public class ConnectionExecutorException : Exception {}

	public class ConnectionExecutorClose : ConnectionExecutorException {
		
	}

	public class ConnectionExecutor
	{
		private IConnectionHandler Handler;
		private Guid ID;
		private bool is_start;

        private event HostEvent onConnect;
        private event HostEvent onDisconnect;

        private ConnectionEventData data;

        public ConnectionExecutor(IConnectionHandler connection_handler, HostEvent onConnect, HostEvent onDisconnect) {
			Handler = connection_handler;
			ID = Guid.NewGuid();
			is_start = true;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
            data = new ConnectionEventData(connection_handler.GetConnetion);
		}

		public void Execute() {
			onConnectEvent();
			while (true) {
				if (is_start) {
					Console.WriteLine("start connection id: {0}", ID.ToString());
					is_start = false;
				}
				else {
					Console.WriteLine("continue connection id: {0}", ID.ToString());
				}
				try
				{
					Handler = Handler.ExecuteHandler();
					if (Handler == null) { break; }
					Handler.Clear();
				}
				catch (ConnectionExecutorClose close) {
					break;
				}
				catch (Exception err) {
					Console.WriteLine("in connection {0} runtime exception {1}", ID, err);
					break;
				}
			}
			onDisConnectEvent();
			Handler.Client.GetStream().Dispose();
			Handler.Client.Close();
			Console.WriteLine("end connection id: {0}", ID.ToString());
		}

        public void onConnectEvent() {
            if (onConnect != null) {
                onConnect(data);
            }
        }
        public void onDisConnectEvent() {
            if (onDisconnect != null) {
                onDisconnect(data);
            }
        }
	}
}
