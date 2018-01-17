using System;

namespace Host.ServerExceptions
{
	public class ExceptionCodeBadRequestFabric : ExceptionFabric
	{
		public ExceptionCodeBadRequestFabric() {}

		public override string name { get { return "Bad Request"; } }

		public override ExceptionCode Create(object data)
		{
			return new BadRequest();
		}
	}

	public class BadRequest : ExceptionCode
	{

		public BadRequest() {
			Code = "400 Bad Request";
			_IsFatal = true;
		}
	}
}
