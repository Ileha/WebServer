using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host
{
    public delegate void HostEvent();
    public interface IHostEvents
    {
        void OnStart();
        void OnStop();
    }
}
