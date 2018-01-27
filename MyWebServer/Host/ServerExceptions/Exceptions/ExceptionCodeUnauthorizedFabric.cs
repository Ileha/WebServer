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

		public override void ExceptionHandle(ref Reqest request, ref Response response) {
			response.AddToHeader("WWW-Authenticate", "Basic realm=\"Access to staging site\"");
		}
    }
}
