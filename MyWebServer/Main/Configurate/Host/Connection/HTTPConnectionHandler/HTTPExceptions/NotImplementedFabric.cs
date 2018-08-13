using ExceptionFabric;
using System;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class NotImplementedFabric : ABSExceptionFabric {
		public NotImplementedFabric() {}

		public override string name { get { return "Not Implemented"; } }

		public override ExceptionCode Create(params object[] data) {
			return new NotImplemented();
		}
	}

	public class NotImplemented  : ExceptionCode {

		public NotImplemented() {
			Code = "501 Not Implemented";
		}
	}
}
