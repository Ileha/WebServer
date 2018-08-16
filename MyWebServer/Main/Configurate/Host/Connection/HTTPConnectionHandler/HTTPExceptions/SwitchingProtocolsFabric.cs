using DataHandlers;
using ExceptionFabric;
using System;
using System.Text;

namespace Configurate.Host.Connection.HTTPConnection.HTTPException
{
	public class SwitchingProtocolsFabric : ABSExceptionFabric {
		public SwitchingProtocolsFabric() {}

		public override string name {
			get {
				return "Switching Protocols";
			}
		}

        public override ExceptionCode Create(params object[] data)
        {
			return new SwitchingProtocols(data[0] as Action, data[1] as string[]);
		}
	}
	public class SwitchingProtocols : ExceptionCode {
		private string[] protocols;
        private Action user_code;
        public SwitchingProtocols(Action userCode, string[] protocols)
        {
			this.protocols = protocols;
			Code = "101 Switching Protocols";
            user_code = userCode;
		}

        public override void ExceptionHandleCode(Response response, IConnetion data)
        {
            StringBuilder sb = new StringBuilder();
			for (int i = 0; i < protocols.Length; i++) {
				if (i == protocols.Length - 1) {
                    sb.Append(protocols[i]);
				}
				else {
					sb.AppendFormat("{0}, ", protocols[i]);
				}
			}
			response.AddToHeader("Upgrade", sb.ToString(), AddMode.rewrite);
            response.AddToHeader("Connection", "Upgrade", AddMode.rewrite);
			response.AddForbiddenHeader("Content-Length");
			response.AddForbiddenHeader("Server");
            user_code();
		}
	}
}
