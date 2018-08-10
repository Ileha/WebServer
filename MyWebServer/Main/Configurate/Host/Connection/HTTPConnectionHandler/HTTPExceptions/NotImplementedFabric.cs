using ExceptionFabric;
using System;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class NotImplementedFabric : ABSExceptionFabric {
		public NotImplementedFabric() {}

		public override string name { get { return "Not Implemented"; } }

		public override ExceptionCode Create(ExceptionUserCode userCode, object data) {
			return new NotImplemented(userCode, (string)data);
		}
	}

	public class NotImplemented  : ExceptionCode {

		public NotImplemented(ExceptionUserCode userCode, string message) : base(userCode) {
			Code = "501 Not Implemented";
		}
	}
}
