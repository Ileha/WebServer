using System;
using Configurate.Host.Connection;
using Configurate.Host;

namespace Events
{
	public delegate void HostEvent(HostEventData data);
	public delegate void ConnectionEvent(ConnectionEventData data);

	public interface OnWebServerStart {
		void OnStart(HostEventData data);
	}
	public interface OnWebServerStop
	{
		void OnStop(HostEventData data);
	}
	public interface OnWebServerConntect
	{
		void OnConntect(ConnectionEventData data);
	}
	public interface OnWebServerDisConntect
	{
		void OnDisConntect(ConnectionEventData data);
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
