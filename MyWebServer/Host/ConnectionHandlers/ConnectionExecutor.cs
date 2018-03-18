using System;
namespace Host.ConnectionHandlers
{
	public class ConnectionExecutor
	{
		private IConnectionHandler Handler;
		private Guid ID;
		private bool is_start;

		public ConnectionExecutor(IConnectionHandler connection_handler) {
			Handler = connection_handler;
			ID = Guid.NewGuid();
			is_start = true;
		}

		public void Execute() {
			while (true) {
				if (is_start) {
					Console.WriteLine("start connection id: {0}", ID.ToString());
					is_start = false;
				}
				else {
					Console.WriteLine("continue connection id: {0}", ID.ToString());
				}
				Handler = Handler.ExecuteHandler();
				if (Handler == null) { break; }
				Handler.Clear();
			}
			Console.WriteLine("end connection id: {0}", ID.ToString());
		}
	}
}
