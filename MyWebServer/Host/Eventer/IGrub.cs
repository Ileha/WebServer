using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host;

namespace Host.Eventer {
    abstract class IGrub : IHostEvents {

        public virtual void OnStart() {}

        public virtual void OnStop() {}
    }
}
