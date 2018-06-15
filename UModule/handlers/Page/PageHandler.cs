using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule;
using System.Reflection;
using UModule.handlers.Page.Controlls;
using System.Xml.Linq;
using CsQuery;

namespace UModule.handlers.Page
{
    public class PageHandler : ABSUModule
    {
        private CQ Page;

        public sealed override void Handle()
        {
            Init();
            Load();
            Render();
            Unload();
        }
        
        public void Init() {
            Page = CQ.CreateDocument(Interact.ReadData);
            FieldInfo[] controlls = this.GetType().GetFields();
            Type master = typeof(ABSElement);
            for (int i = 0; i < controlls.Length; i++) {
                Type need_type = controlls[i].DeclaringType;
                if (!need_type.IsSubclassOf(master)) { continue; }
                ABSElement element = (ABSElement)Activator.CreateInstance(need_type);
                element.Init(Page[string.Format("[title=\"{0}\"", controlls[i].Name)]);
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
