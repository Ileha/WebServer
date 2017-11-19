using System;
using System.Text;

namespace Host.ServerExceptions
{
	public abstract class ExceptionCode : Exception
	{
		protected string Code;
		protected bool _IsFatal;
		public bool IsFatal
		{
			get { return _IsFatal; }
		}

		public byte[] GetAddingBytesToHeader()
		{
			return Encoding.UTF8.GetBytes(GetAddingToHeader());
		}
		public byte[] GetAddingBytesToData()
		{
			return Encoding.UTF8.GetBytes(GetAddingToData());
		}

		protected abstract string GetAddingToHeader();
		protected virtual string GetAddingToData() {
			return "<html><body><h2>An error has occurred code of error " + Code + "</h2></body></html>";
		}

		public string GetExeptionCode()
		{
			return Code;
		}

		//        public static ExceptionCode BadRequest() {
		//            ExceptionCode res = new ExceptionCode(true);
		//            res.Code = "400 Bad Request";
		//            return res;
		//        }
		//        public static ExceptionCode OK() {
		//            ExceptionCode res = new ExceptionCode(false);
		//            res.Code = "200 OK";
		//            return res;
		//        }
		//        public static ExceptionCode NotFound() {
		//            ExceptionCode res = new ExceptionCode(true);
		//            res.Code = "404 Not Found";
		//            return res;
		//        }
		//        public static ExceptionCode InternalServerError() {
		//            ExceptionCode res = new ExceptionCode(true);
		//            res.Code = "500 Internal Server Error";
		//            return res;
		//        }
		//        public static ExceptionCode MovedPermanently(string new_url, string host) {
		//            ExceptionCode res = new ExceptionCode(true);
		//            res.Code = @"HTTP/1.1 301 Moved Permanently
		//Location: "+host+new_url;
		//            return res;
		//        }

	}

	public class BadRequest : ExceptionCode
	{

		public BadRequest()
		{
			Code = "400 Bad Request";
			_IsFatal = true;
		}

		protected override string GetAddingToHeader()
		{
			throw new NotImplementedException();
		}
	}

	public class OK : ExceptionCode
	{

		public OK()
		{
			Code = "200 OK";
			_IsFatal = false;
		}

		protected override string GetAddingToHeader()
		{
			throw new NotImplementedException();
		}
	}

	public class NotFound : ExceptionCode
	{

		public NotFound()
		{
			Code = "404 Not Found";
			_IsFatal = true;
		}

		protected override string GetAddingToHeader()
		{
			throw new NotImplementedException();(
		}
	}

	public class InternalServerError : ExceptionCode
	{

		public InternalServerError() {
			Code = "500 Internal Server Error";
			_IsFatal = true;
		}

		protected override string GetAddingToHeader() {
			throw new NotImplementedException();
		}
	}
				
}
