using System;
using Host.Eventer;
using Host.ConnectionHandlers.ExecutorExceptions;

namespace Host.ConnectionHandlers
{
	public class ConnectionExecutor
	{
		private IConnectionHandler Handler;
		private Guid ID;

        private event ConnectionEvent onConnect;
        private event ConnectionEvent onDisconnect;

        public ConnectionExecutor(IConnectionHandler connection_handler, ConnectionEvent onConnect, ConnectionEvent onDisconnect) {
			Handler = connection_handler;
			ID = Guid.NewGuid();
            this.onConnect = onConnect;
            this.onDisconnect = onDisconnect;
		}

        private void ConnectionExecute(ref IConnectionHandler ConnectHandler)
        {
            ConnectionEventData data = new ConnectionEventData(ConnectHandler.GetEventConnetion);
            onConnectEvent(data);
            do {
                ConnectHandler.Reset();
                ConnectHandler.Execute();
                Console.WriteLine("continue connection id: {0}", ID.ToString());
            }while(ConnectHandler == ConnectHandler.ExecuteHandler);
            onDisconnect(data);
            ConnectHandler = ConnectHandler.ExecuteHandler;
        }

		public void Execute() {
            Console.WriteLine("start connection id: {0}", ID.ToString());
			try {
                while (true) { 
                    ConnectionExecute(ref Handler);
                }
			}
			catch (ConnectionExecutorClose close) {}
			catch (Exception err) {
				Console.WriteLine("in connection {0} runtime exception {1}", ID, err);
			}
            Handler.Dispose();
			Console.WriteLine("end connection id: {0}", ID.ToString());
		}

        public void onConnectEvent(ConnectionEventData data)
        {
            if (onConnect != null) {
                onConnect(data);
            }
        }
        public void onDisConnectEvent(ConnectionEventData data)
        {
            if (onDisconnect != null) {
                onDisconnect(data);
            }
        }
	}
}
