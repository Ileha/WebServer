using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host;

namespace Host
{
    public delegate void HostEvent(WebSerwer host);
    public interface IHostEvents
    {
        void OnStart(WebSerwer host);
        void OnStop(WebSerwer host);
    }
}
