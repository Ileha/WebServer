using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host;
using Host.ConnectionHandlers;

namespace Host
{
    public delegate void HostEvent(WebSerwer host);
    public delegate void ConnectEvent(IConnetion host);

    public abstract class IHostEvents
    {
        public virtual void OnStart(WebSerwer host) { }
        public virtual void OnStop(WebSerwer host) { }

        public virtual void OnConntect(IConnetion host) { }
        public virtual void OnDisConntect(IConnetion host) { }

    }
}
