using System;
using Host.ConnectionHandlers;

namespace Host.Eventer
{
	public delegate void HostEvent(EventArgs data);

	public interface OnWebServerStart
	{
		void OnStart(EventArgs data);
	}
	public interface OnWebServerStop
	{
		void OnStop(EventArgs data);
	}
	public interface OnWebServerConntect
	{
		void OnConntect(EventArgs data);
	}
	public interface OnWebServerDisConntect
	{
		void OnDisConntect(EventArgs data);
	}


	public class HostEventData : EventArgs {
		public WebSerwer host;

		public HostEventData(WebSerwer host) { this.host = host; }
	}
	public class ConnectionEventData : EventArgs {
		public IConnetion connect;

		public ConnectionEventData(IConnetion connect) { this.connect = connect; }	
	}
}
