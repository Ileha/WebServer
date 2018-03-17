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
			return new Unauthorized((string)data);
        }
    }
    public class Unauthorized : ExceptionCode
    {
		private string message;

        public Unauthorized(string message) {
            Code = "401 Unauthorized";
            _IsFatal = true;
			this.message = message;
        }

		public override void ExceptionHandle(ref Reqest request, ref Response response) {
			response.AddToHeader("WWW-Authenticate", "Basic realm=\""+message+"\"");
		}
    }
}
