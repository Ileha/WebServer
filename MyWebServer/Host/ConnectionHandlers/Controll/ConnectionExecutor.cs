﻿using System;
using Host.Eventer;
using Host.ConnectionHandlers.ExecutorExceptions;

namespace Host.ConnectionHandlers
{
	public class ConnectionExecutor
	{
		private IConnectionHandler Handler;
		private Guid ID;
		private bool is_start;

        private event ConnectionEvent onConnect;
        private event ConnectionEvent onDisconnect;

        private ConnectionEventData data;

        public ConnectionExecutor(IConnectionHandler connection_handler, ConnectionEvent onConnect, ConnectionEvent onDisconnect) {
			Handler = connection_handler;
			ID = Guid.NewGuid();
			is_start = true;
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
            data = new ConnectionEventData(connection_handler.GetConnetion);
		}

		public void Execute() {
            Console.WriteLine("start connection id: {0}", ID.ToString());
			onConnectEvent();
            IConnectionHandler next_handler = null;
			while (true) {
				try {
                    Handler.Execute();
                    next_handler = Handler.ExecuteHandler;
                    if (Handler == next_handler) {
                        next_handler.Reset();//сброс для следующих данных
                    }
                    else {
                        onDisConnectEvent();
                        data = new ConnectionEventData(next_handler.GetConnetion);
                        onConnectEvent();
                    }
                    Handler = next_handler;
                    if (is_start) {
                        is_start = false;
                    }
                    else {
                        Console.WriteLine("continue connection id: {0}", ID.ToString());
                    }
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
            Handler.Dispose();
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
