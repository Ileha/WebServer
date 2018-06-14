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
        private int i;

        public TestHandler() {
            i = 0;
        }
        public override void Handle()
        {
            Interact.ReadData.CopyTo(Interact.OutputData);
            Write(string.Format("\nyou send {0} times\n", i));
            Interact.InputData.CopyTo(Interact.OutputData);
            i++;
        }
    }
}
