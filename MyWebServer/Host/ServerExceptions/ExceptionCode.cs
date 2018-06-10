using System;
using System.Text;
using Host.ConnectionHandlers;
using Host.MIME;

namespace Host.ServerExceptions
{
    public delegate void ExceptionUserCode(ABSMIME Handler, Reqest request, Response response, IConnetion handler, Action<ABSMIME, Reqest, Response, IConnetion> Base);

	public abstract class ExceptionCode : Exception
	{
		protected string Code;
        protected ExceptionUserCode userCode;

        public ExceptionCode(ExceptionUserCode userCode) {
            this.userCode = userCode;
        }

        public void ExceptionHandle(ABSMIME Handler, Reqest request, Response response, IConnetion data)
        {
            if (userCode != null) {
                userCode.Invoke(Handler, request, response, data, ExceptionHandleCode);
            }
            else {
                ExceptionHandleCode(Handler, request, response, data);
            }
		}

        public virtual void ExceptionHandleCode(ABSMIME Handler, Reqest request, Response response, IConnetion data) {
            response.Clear();
            response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
			byte[] _data = Encoding.UTF8.GetBytes("<html><body><h2>An error has occurred code of error " + Code + "</h2></body></html>");
            data.OutputData.Write(_data, 0, _data.Length);
        }

		public string GetExeptionCode() {
			return Code;
		}
	}		
}
