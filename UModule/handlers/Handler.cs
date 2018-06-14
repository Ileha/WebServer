using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule;

namespace UModule.handlers
{
    public class Handler : ABSUModule
    {
        public override string ContentType
        {
            get { return "text/plain; charset=UTF-8"; }
        }

        public override void Handle() {}
    }
}
