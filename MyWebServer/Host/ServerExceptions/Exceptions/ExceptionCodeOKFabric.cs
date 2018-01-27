using System;

namespace Host.ServerExceptions
{
	public class ExceptionCodeOKFabric : ExceptionFabric
	{
		public ExceptionCodeOKFabric() {}

		public override string name { get { return "OK"; } }

		public override ExceptionCode Create(object data) {
			return new OK();
		}
	}

	public class OK : ExceptionCode {

		public OK() {
			Code = "200 OK";
			_IsFatal = false;
		}

		public override void ExceptionHandle(ref Reqest request, ref Response response) {}
	}
}
