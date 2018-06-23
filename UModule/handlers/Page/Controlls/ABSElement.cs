using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace UModule.handlers.Page.Controlls
{
    public abstract class ABSElement
    {
        private HtmlNode block;
        public string id { get; private set; }
        public void Render() {
            
            block = OnRender(block);
        }
        public void Init(HtmlNode cQ) {
            id = cQ.Attributes["id"].Value;
            block = OnInit(cQ);
        }
        protected abstract HtmlNode OnInit(HtmlNode cQ);
        protected abstract HtmlNode OnRender(HtmlNode out_node);
    }
}
