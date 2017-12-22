using System;
using System.Text;
using Host.DataHeaderInterfaces;

namespace Host.ServerExceptions
{
	public abstract class ExceptionCode : Exception, IAddData, IAddHeader
	{
		protected string Code;
		protected bool _IsFatal;
		public bool IsFatal
		{
			get { return _IsFatal; }
		}

        public virtual void GetAddingToHeader(Action<string, string> add_to_header) {
            add_to_header("Content-Type", "text/html; charset=UTF-8");
        }
        public byte[] GetByteData() {
            return Encoding.UTF8.GetBytes(GetDataString());
        }
		public virtual string GetDataString() {
			return "<html><body><h2>An error has occurred code of error " + Code + "</h2></body></html>";
		}

		public string GetExeptionCode()
		{
			return Code;
		}
	}

	public class BadRequest : ExceptionCode
	{

		public BadRequest()
		{
			Code = "400 Bad Request";
			_IsFatal = true;
		}
	}

	public class OK : ExceptionCode
	{

		public OK()
		{
			Code = "200 OK";
			_IsFatal = false;
		}
	}

	public class NotFound : ExceptionCode
	{

		public NotFound()
		{
			Code = "404 Not Found";
			_IsFatal = true;
		}
	}

	public class InternalServerError : ExceptionCode
	{

		public InternalServerError() {
			Code = "500 Internal Server Error";
			_IsFatal = true;
		}
	}
    public class MovedPermanently : ExceptionCode {
        private string targeURL;

        public MovedPermanently(string targeURL)
        {
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
