using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UModule.handlers.Page.Controlls
{
    public class ABSElement
    {
        public abstract void Init(XElement el);
        public abstract void Render(ABSUModule handler);//отображение
    }
}
