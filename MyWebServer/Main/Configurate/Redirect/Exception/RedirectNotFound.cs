using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Redirect.Exception
{
    public class RedirectNotFound : System.Exception
    {
        private string url_value;

        public RedirectNotFound(string url)
        {
            url_value = url;
        }

        public override string ToString()
        {
            return string.Format("to url {0} does not found redirect", url_value);
        }
    }
}
