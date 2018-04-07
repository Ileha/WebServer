using System;
using System.Text;
using Host.ConnectionHandlers;

namespace Host.ServerExceptions
{
    public delegate void ExceptionUserCode(ref Reqest request, ref Response response, IConnetion handler);

	public abstract class ExceptionCode : Exception
	{
		protected string Code;
		protected bool _IsFatal;
		public bool IsFatal {
			get { return _IsFatal; }
		}
        protected ExceptionUserCode userCode;

        public ExceptionCode(ExceptionUserCode userCode) {
            this.userCode = userCode;
        }

		public void ExceptionHandle(ref Reqest request, ref Response response, IConnetion handler) {
            ExceptionHandleCode(ref request, ref response, handler);
            if (userCode != null) {
                userCode.Invoke(ref request, ref response, handler);
            }
		}

        public virtual void ExceptionHandleCode(ref Reqest request, ref Response response, IConnetion handler) {
            response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
			byte[] data = Encoding.UTF8.GetBytes("<html><body><h2>An error has occurred code of error " + Code + "</h2></body></html>");
			handler.OutputData.Write(data, 0, data.Length);
        }

		public string GetExeptionCode() {
			return Code;
		}
	}		
}
