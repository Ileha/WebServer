using System;

namespace Events {
	public abstract class ABSGrub : OnWebServerStart, OnWebServerStop, OnWebServerConntect, OnWebServerDisConntect {
		public virtual void OnConntect(ConnectionEventData data) {}
		public virtual void OnDisConntect(ConnectionEventData data){}
		public virtual void OnStart(HostEventData data) {}
		public virtual void OnStop(HostEventData data) {}
	}
}