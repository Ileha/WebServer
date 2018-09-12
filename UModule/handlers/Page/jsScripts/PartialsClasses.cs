using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule.handlers.Page.Controlls;
using UModule;

namespace UModule.handlers.Page.jsScripts
{
    public partial class Serialize {
        List<ABSElement> elements;
        public Serialize(List<ABSElement> elements) {
            this.elements = elements;
        }
    }

    public partial class Connect {
        Interactive connect;
        public Connect(Interactive interact) {
            connect = interact;
        }
    }
}
