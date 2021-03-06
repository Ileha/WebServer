﻿using System;

namespace ExceptionFabric
{
	public abstract class ABSExceptionFabric {
		public abstract string name { get; }

		public ABSExceptionFabric() {}

		public abstract ExceptionCode Create(params object[] data);
	}
}
