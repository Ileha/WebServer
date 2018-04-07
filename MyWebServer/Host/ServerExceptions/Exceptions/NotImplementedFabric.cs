using System;

namespace Host.ServerExceptions {
	public class NotImplementedFabric : ExceptionFabric {
		public NotImplementedFabric() {}

		public override string name { get { return "Not Implemented"; } }

		public override ExceptionCode Create(ExceptionUserCode userCode, object data) {
			return new NotImplemented(userCode, (string)data);
		}
	}

	public class NotImplemented  : ExceptionCode {

		public NotImplemented(ExceptionUserCode userCode, string message) : base(userCode) {
			Code = "501 Not Implemented";
			_IsFatal = true;
		}
	}
}
