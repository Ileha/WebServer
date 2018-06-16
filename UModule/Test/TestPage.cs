using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UModule.handlers.Page;
using UModule.handlers.Page.Controlls;

namespace UModule.Test
{
    public class TestPage : PageHandler {
        public UButton button;
        public UText textbox;

        public override void Load() {
            textbox.Text = "This page generate from code";
        }
    }
}
