using ExceptionFabric;
using System;
namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class ExceptionCodeNotFoundFabric : ABSExceptionFabric
	{
		public ExceptionCodeNotFoundFabric() {}

		public override string name { get { return "Not Found"; } }

        public override ExceptionCode Create(params object[] data)
		{
			return new NotFound();
		}
	}

	public class NotFound : ExceptionCode {
        public NotFound() {
			Code = "404 Not Found";
		}	
	}
}
