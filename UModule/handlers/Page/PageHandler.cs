using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule;
using System.Reflection;
using UModule.handlers.Page.Controlls;
using System.Xml.Linq;

namespace UModule.handlers.Page
{
    public class PageHandler : ABSUModule
    {
        private XDocument Page;

        public sealed override void Handle()
        {
            Init();
            Load();
            Render();
            Unload();
        }
        
        public void Init() {
            //код
            Page = XDocument.Load(Interact.ReadData);
            FieldInfo[] controlls = this.GetType().GetFields();
            for (int i = 0; i < controlls.Length; i++) {
                Type need_type = controlls[i].DeclaringType;
                ABSElement element = (ABSElement)Activator.CreateInstance(need_type);
                //element.Init();
                controlls[i].SetValue(this, element);

            }
            OnInit();
        }
        public virtual void OnInit() { }
        public virtual void Load() { }
        public void Render() {
        
        }
        public void Unload() {
            
        }
        //Init
        //Load
        //PreRender
        //Unload
        public override string ContentType
        {
            get { return "text/html; charset=UTF-8"; }
        }
    }
}
