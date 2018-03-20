using System;

namespace Host.ServerExceptions
{
	public class ExceptionCodeBadRequestFabric : ExceptionFabric
	{
		public ExceptionCodeBadRequestFabric() {}

		public override string name { get { return "Bad Request"; } }

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
		{
			return new BadRequest(userCode);
		}
	}

	public class BadRequest : ExceptionCode
	{

        public BadRequest(ExceptionUserCode userCode)
            : base(userCode)
        {
			Code = "400 Bad Request";
			_IsFatal = true;
		}
	}
}
