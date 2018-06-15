using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsQuery;

namespace UModule.handlers.Page.Controlls
{
    public abstract class ABSElement
    {
        public abstract void Render(ABSUModule handler);//отображение

        public abstract void Init(CQ cQ);
    }
}
