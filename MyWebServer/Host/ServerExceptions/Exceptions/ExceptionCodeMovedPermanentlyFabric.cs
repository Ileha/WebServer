﻿using System;
using Host.ConnectionHandlers;

namespace Host.ServerExceptions
{
	public class ExceptionCodeMovedPermanentlyFabric : ExceptionFabric
	{
		public ExceptionCodeMovedPermanentlyFabric() {}

		public override string name { get { return "Moved Permanently"; } }

		public override ExceptionCode Create(object data)
		{
			return new MovedPermanently((string)data);
		}
	}

	public class MovedPermanently : ExceptionCode {
		private string targeURL;

		public MovedPermanently(string targeURL) {
			Code = "301 Moved Permanently";
			_IsFatal = true;
			this.targeURL = targeURL;
		}
		public override void ExceptionHandle(ref Reqest request, ref Response response) {
			response.AddToHeader("Location", targeURL, AddMode.rewrite);
		}
	}
}
