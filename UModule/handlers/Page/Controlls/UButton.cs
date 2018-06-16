using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UModule.handlers.Page.Controlls 
{
    public class UButton : ABSElement
    {
        public string Test;

        protected override HtmlNode OnInit(HtmlNode domObject) {
            HtmlNode newNode = domObject.CloneNode("button");
            domObject.ParentNode.ReplaceChild(newNode, domObject);
            Test = newNode.InnerText;
            return newNode;
        }

        protected override HtmlNode OnRender(HtmlNode out_node) {
            out_node.InnerHtml = Test;
            return out_node;
        }
    }
}
