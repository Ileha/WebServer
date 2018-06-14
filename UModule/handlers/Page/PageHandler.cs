using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule;

namespace UModule.handlers.Page
{
    public class PageHandler : ABSUModule
    {
        public sealed override void Handle()
        {
            Init();
            Load();
            Render();
            Unload();
        }
        
        public void Init() {
            //код
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
