using System;
namespace Host.ServerExceptions
{
	public class ExceptionCodeInternalServerErrorFabric : ExceptionFabric {
		public ExceptionCodeInternalServerErrorFabric() {}

		public override string name { get { return "Internal Server Error"; } }

		public override ExceptionCode Create(object data)
		{
			return new InternalServerError();
		}
	}

	public class InternalServerError : ExceptionCode {
		public InternalServerError() {
			Code = "500 Internal Server Error";
			_IsFatal = true;
		}	
	}
}
