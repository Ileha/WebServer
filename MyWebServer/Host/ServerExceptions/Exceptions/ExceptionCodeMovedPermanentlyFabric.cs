using System;
using Host.ConnectionHandlers;

namespace Host.ServerExceptions
{
	public class ExceptionCodeMovedPermanentlyFabric : ExceptionFabric
	{
		public ExceptionCodeMovedPermanentlyFabric() {}

		public override string name { get { return "Moved Permanently"; } }

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
		{
			return new MovedPermanently((string)data,userCode);
		}
	}

	public class MovedPermanently : ExceptionCode {
		private string targeURL;

        public MovedPermanently(string targeURL, ExceptionUserCode userCode)
            : base(userCode)
        {
			Code = "301 Moved Permanently";
			_IsFatal = true;
			this.targeURL = targeURL;
		}
        public override void ExceptionHandleCode(ref Reqest request, ref Response response, IConnetion handler) {
			response.AddToHeader("Location", targeURL, AddMode.rewrite);
		}
	}
}
