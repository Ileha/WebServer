﻿using System;
namespace Host.ServerExceptions
{
	public class ExceptionCodeNotFoundFabric : ExceptionFabric
	{
		public ExceptionCodeNotFoundFabric() {}

		public override string name { get { return "Not Found"; } }

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
		{
			return new NotFound(userCode);
		}
	}

	public class NotFound : ExceptionCode {
        public NotFound(ExceptionUserCode userCode)
            : base(userCode)
        {
			Code = "404 Not Found";
			_IsFatal = true;
		}	
	}
}