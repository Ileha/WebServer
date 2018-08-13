using ExceptionFabric;
using System;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class ExceptionCodeBadRequestFabric : ABSExceptionFabric
	{
		public ExceptionCodeBadRequestFabric() {}

		public override string name { get { return "Bad Request"; } }

        public override ExceptionCode Create(params object[] data)
		{
			return new BadRequest();
		}
	}

	public class BadRequest : ExceptionCode
	{
        public BadRequest() {
			Code = "400 Bad Request";
        }
	}
}
