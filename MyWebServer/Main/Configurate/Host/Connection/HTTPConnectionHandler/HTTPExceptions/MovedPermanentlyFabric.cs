using DataHandlers;
using ExceptionFabric;
using System;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class ExceptionCodeMovedPermanentlyFabric : ABSExceptionFabric
	{
		public ExceptionCodeMovedPermanentlyFabric() {}

		public override string name { get { return "Moved Permanently"; } }

        public override ExceptionCode Create(params object[] data)
		{
			return new MovedPermanently((string)data[0]);
		}
	}

	public class MovedPermanently : ExceptionCode {
		private string targeURL;

        public MovedPermanently(string targeURL) {
			Code = "301 Moved Permanently";
			this.targeURL = targeURL;
		}
        public override void ExceptionHandleCode(Response response, IConnetion data)
        {
			response.AddToHeader("Location", targeURL, AddMode.rewrite);
		}
	}
}
