using ExceptionFabric;
using System;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class ExceptionCodeInternalServerErrorFabric : ABSExceptionFabric {
		public ExceptionCodeInternalServerErrorFabric() {}

		public override string name { get { return "Internal Server Error"; } }

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
		{
			return new InternalServerError(userCode);
		}
	}

	public class InternalServerError : ExceptionCode {
        public InternalServerError(ExceptionUserCode userCode)
            : base(userCode)
        {
			Code = "500 Internal Server Error";
		}	
	}
}
