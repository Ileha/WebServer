using ExceptionFabric;
using System;
namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class ExceptionCodeNotFoundFabric : ABSExceptionFabric
	{
		public ExceptionCodeNotFoundFabric() {}

		public override string name { get { return "Not Found"; } }

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
		{
			return new NotFound(userCode);
		}
	}

	public class NotFound : ExceptionCode {
        public NotFound(ExceptionUserCode userCode)
            : base(userCode)
        {
			Code = "404 Not Found";
		}	
	}
}
