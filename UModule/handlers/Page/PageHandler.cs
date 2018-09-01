using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule;
using System.Reflection;
using UModule.handlers.Page.Controlls;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace UModule.handlers.Page
{
    public class PageHandler : ABSUModule
    {
        private HtmlDocument Page;
        private List<ABSElement> elements;

        public sealed override void Handle()
        {
            Init();
            Load();
            Render();
            Unload();
        }
        
        public void Init() {
            Page = new HtmlDocument();
            Page.Load(Interact.ReadData);
            elements = new List<ABSElement>();
            FieldInfo[] controlls = this.GetType().GetFields();
            Type master = typeof(ABSElement);
            for (int i = 0; i < controlls.Length; i++) {
                Type need_type = controlls[i].FieldType;
                if (!need_type.IsSubclassOf(master)) { continue; }
                ABSElement element = (ABSElement)Activator.CreateInstance(need_type);
                element.Init(Page.DocumentNode.SelectSingleNode(string.Format("//*[@id=\"{0}\"]", controlls[i].Name)));
                controlls[i].SetValue(this, element);
                elements.Add(element);
            }
            OnInit();
        }
        public virtual void OnInit() { }
        public virtual void Load() { }
        public void Render() {
            HtmlNode body = Page.DocumentNode.SelectSingleNode("/html/body");
            HtmlNode script = Page.CreateElement("script");
            script.Attributes.Add("type", "text/javascript");
            body.AppendChild(script);
            StringBuilder sb = new StringBuilder();
            sb.Append("var ids = [");
            int count = elements.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != count-1) {
                    sb.AppendFormat("\"{0}\",", elements[i].id);
                }
                else {
                    sb.AppendFormat("\"{0}\"", elements[i].id);
                }
                elements[i].Render();
            }
            sb.Append("];\r\n");
            sb.Append("var s = new XMLSerializer();\r\nvar str = \"<res>\";\r\nfor (i = 0; i < ids.length; i++) {\r\nstr+=s.serializeToString(document.getElementById(ids[i]));\r\n}\r\nstr+=\"</res>\";\r\nalert(str);\r\n");
            script.InnerHtml = sb.ToString();
            
            script = Page.CreateElement("script");
            script.Attributes.Add("type", "text/javascript");
            body.AppendChild(script);
            sb.Clear();
            sb.Append("var socket;\r\nfunction connect() {\r\nsocket = new WebSocket(\"ws://");
            sb.AppendFormat("{0}{1}", Interact.LocalEndPoint, Interact.URL);
            sb.Append("\");\r\nsocket.onopen = function() {\r\nconsole.log(\"Соединение установлено.\");\r\n};\r\nsocket.onclose = function(event) {\r\nif (event.wasClean) {\r\nconsole.log(\"Соединение закрыто чисто\");\r\n} else {\r\nconsole.log(\"Обрыв соединения\");\r\n}\r\nconsole.log(\"Код: \" + event.code + \" причина: \" + event.reason);\r\n};\r\nsocket.onmessage = function(event) {\r\nconsole.log(\"Получены данные \" + event.data);\r\n};\r\nsocket.onerror = function(error) {\r\n console.log(\"Ошибка \" + error.message);\r\n};\r\n}");
            script.InnerHtml = sb.ToString();
        }
        public void Unload() {
            string res = Page.DocumentNode.InnerHtml;
            byte[] buffer = Encoding.UTF8.GetBytes(res);
            Interact.OutputData.Write(buffer, 0, buffer.Length);
        }
        public override string ContentType
        {
            get { return "text/html; charset=UTF-8"; }
        }
    }
}
