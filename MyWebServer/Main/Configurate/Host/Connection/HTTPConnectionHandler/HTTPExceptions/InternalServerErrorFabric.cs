using ExceptionFabric;
using System;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class ExceptionCodeInternalServerErrorFabric : ABSExceptionFabric {
		public ExceptionCodeInternalServerErrorFabric() {}

		public override string name { get { return "Internal Server Error"; } }

        public override ExceptionCode Create(params object[] data)
		{
			return new InternalServerError();
		}
	}

	public class InternalServerError : ExceptionCode {
        public InternalServerError() {
			Code = "500 Internal Server Error";
		}	
	}
}
