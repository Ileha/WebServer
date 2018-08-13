using DataHandlers;
using ExceptionFabric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
    public class ExceptionCodeUnauthorizedFabric : ABSExceptionFabric
    {
        public override string name
        {
            get { return "Unauthorized"; }
        }

        public override ExceptionCode Create(params object[] data)
        {
			return new Unauthorized((string)data[0]);
        }
    }
    public class Unauthorized : ExceptionCode
    {
		private string message;

        public Unauthorized(string message)
        {
            Code = "401 Unauthorized";
			this.message = message;
        }

        public override void ExceptionHandleCode(ABSMIME Handler, Reqest request, Response response, IConnetion handler)
        {
			response.AddToHeader("WWW-Authenticate", "Basic realm=\""+message+"\"", AddMode.rewrite);
		}
    }
}
