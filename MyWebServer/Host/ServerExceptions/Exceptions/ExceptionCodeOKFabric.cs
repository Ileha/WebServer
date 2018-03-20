using System;
using Host.ConnectionHandlers;

namespace Host.ServerExceptions
{
	public class ExceptionCodeOKFabric : ExceptionFabric
	{
		public ExceptionCodeOKFabric() {}

		public override string name { get { return "OK"; } }

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
        {
			return new OK(userCode);
		}
	}

	public class OK : ExceptionCode {

        public OK(ExceptionUserCode userCode)
            : base(userCode)
        {
			Code = "200 OK";
			_IsFatal = false;
		}

		public override void ExceptionHandleCode(ref Reqest request, ref Response response) {}
	}
}
