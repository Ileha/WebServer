using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.ConnectionHandlers;

namespace Host.ServerExceptions
{
    public class ExceptionCodeUnauthorizedFabric : ABSExceptionFabric
    {
        public override string name
        {
            get { return "Unauthorized"; }
        }

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
        {
			return new Unauthorized((string)data, userCode);
        }
    }
    public class Unauthorized : ExceptionCode
    {
		private string message;

        public Unauthorized(string message, ExceptionUserCode userCode)
            : base(userCode)
        {
            Code = "401 Unauthorized";
			this.message = message;
        }

        public override void ExceptionHandleCode(MIME.ABSMIME Handler, Reqest request, Response response, IConnetion handler)
        {
			response.AddToHeader("WWW-Authenticate", "Basic realm=\""+message+"\"", AddMode.rewrite);
		}
    }
}
