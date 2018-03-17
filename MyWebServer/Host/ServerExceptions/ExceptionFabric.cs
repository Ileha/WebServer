using System;

namespace Host.ServerExceptions
{
	public abstract class ExceptionFabric {
		public abstract string name { get; }

		public ExceptionFabric() {}

		public abstract ExceptionCode Create(object data);
	}
}
