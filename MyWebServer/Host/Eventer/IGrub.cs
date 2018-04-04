using System;
using Host.ConnectionHandlers;

namespace Host.Eventer {
	public abstract class IGrub : OnWebServerStart, OnWebServerStop, OnWebServerConntect, OnWebServerDisConntect {
		public virtual void OnConntect(EventArgs data) {}

		public virtual void OnDisConntect(EventArgs data) {}

		public virtual void OnStart(EventArgs data) {}

		public virtual void OnStop(EventArgs data) {}
	}
}