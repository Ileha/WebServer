using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule.handlers;
using UModule;

namespace UModule.Test
{
    public class TestHandler : Handler
    {
        public override void Load() {
            Interact.ReadData.CopyTo(Interact.OutputData);
            Interact.InputData.CopyTo(Interact.OutputData);
        }
    }
}
