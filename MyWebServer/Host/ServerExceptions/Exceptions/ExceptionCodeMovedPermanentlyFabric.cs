using System;

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
		public override string GetDataString() {
			return "";
		}
		public override void GetAddingToHeader(Action<string, string> add_to_header) {
			add_to_header("Location", targeURL);
		} 
	}
}
