using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule;

namespace UModule.handlers
{
    public class PageHandler : ABSUModule
    {
        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Render()
        {
            throw new NotImplementedException();
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }

        public override string ContentType
        {
            get { return "text/html; charset=UTF-8"; }
        }
    }
}
