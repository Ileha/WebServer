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
        private CQ block;
        public abstract void Render(ABSUModule handler);//отображение
        public void Init(CQ cQ) {
            block = cQ;
            OnInit(cQ);
        }
        protected abstract void OnInit(CQ cQ);
    }
}
