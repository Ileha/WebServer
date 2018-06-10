using System;
using Host.ConnectionHandlers;
using System.Text;

namespace Host.ServerExceptions
{
	public class ExceptionCodeOKFabric : ABSExceptionFabric
	{
		public ExceptionCodeOKFabric() {}

		public override string name { get { return "OK"; } }

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
        {
			return new OK(userCode);
		}
	}

	public class OK : ExceptionCode {

        public OK(ExceptionUserCode userCode)
            : base(userCode)
        {
			Code = "200 OK";
		}

        public override void ExceptionHandleCode(MIME.ABSMIME Handler, Reqest request, Response response, IConnetion data)
        {
            try {
                Handler.Headers(response, request, data.ReadData);
                Handler.Handle(data);
            }
            catch (ExceptionCode err) {
                throw err;
            }
            catch (Exception err) {
                response.Clear();
                response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
                byte[] exce = Encoding.UTF8.GetBytes(err.ToString());
                data.OutputData.Write(exce, 0, exce.Length);
            }
        }
	}
}
