using System;
using RequestHandlers;
using System.Text;
using ExceptionFabric;
using DataHandlers;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class ExceptionCodeOKFabric : ABSExceptionFabric
	{
		public ExceptionCodeOKFabric() {}
		public override string name { get { return "OK"; } }
        public override ExceptionCode Create(params object[] data) {
            return new OK();
        }
    }

	public class OK : ExceptionCode {

        public OK() {
			Code = "200 OK";
		}

        public override void ExceptionHandleCode(ABSMIME Handler, Reqest request, Response response, IConnetion data)
        {
            try {
                Action<Response, Reqest> headers;
                Handler.Handle(data, out headers);
                headers(response, request);
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
