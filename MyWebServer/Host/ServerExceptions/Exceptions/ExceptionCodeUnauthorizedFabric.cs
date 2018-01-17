using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host.ServerExceptions
{
    public class ExceptionCodeUnauthorizedFabric : ExceptionFabric
    {
        public override string name
        {
            get { return "Unauthorized"; }
        }

        public override ExceptionCode Create(object data)
        {
            throw new NotImplementedException();
        }
    }
    public class Unauthorized : ExceptionCode
    {
        public Unauthorized() {
            Code = "401 Unauthorized";
            _IsFatal = true;
        }

        public override void GetAddingToHeader(Action<string, string> add_to_header) {
            add_to_header("WWW-Authenticate", "Basic realm=\"Access to staging site\"");
            base.GetAddingToHeader(add_to_header);
        }
    }
}
