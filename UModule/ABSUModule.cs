using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.ConnectionHandlers;
using System.Xml.Linq;

namespace UModule
{
    public abstract class ABSUModule
    {
        public Interactive Interact { get; private set; }
        public void Build(IConnetion Interact, XElement data) {
            this.Interact = new Interactive(Interact, data);
        }
        public abstract void Init();//инициализация контроллов
        public virtual void Load() {}
        public virtual void PreRender() {}
        public abstract void Render();//запись в поток
        public abstract void Unload();//конец
        public abstract string ContentType { get; }
    }
}
