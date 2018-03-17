using System;
namespace Host.ServerExceptions
{
	public class ExceptionCodeNotFoundFabric : ExceptionFabric
	{
		public ExceptionCodeNotFoundFabric() {}

		public override string name { get { return "Not Found"; } }

		public override ExceptionCode Create(object data)
		{
			return new NotFound();
		}
	}

	public class NotFound : ExceptionCode {
		public NotFound() {
			Code = "404 Not Found";
			_IsFatal = true;
		}	
	}
}
