using System;

namespace Host.ServerExceptions
{
	public abstract class ABSExceptionFabric {
		public abstract string name { get; }

		public ABSExceptionFabric() {}

		public abstract ExceptionCode Create(ExceptionUserCode userCode, object data);
	}
}
