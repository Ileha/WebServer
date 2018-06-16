using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace UModule.handlers.Page.Controlls
{
    public class UText : ABSElement {
        public string Text;
        protected override HtmlNode OnInit(HtmlNode domObject)
        {

            HtmlNode newNode = domObject.CloneNode("p");
            domObject.ParentNode.ReplaceChild(newNode, domObject);
            Text = newNode.InnerText;
            return newNode;
        }

        protected override HtmlNode OnRender(HtmlNode out_node)
        {
            out_node.InnerHtml = Text;
            return out_node;
        }
    }
}
