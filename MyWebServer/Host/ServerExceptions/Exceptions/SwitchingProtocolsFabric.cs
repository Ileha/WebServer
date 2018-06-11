using Host.ConnectionHandlers;
using System.Text;

namespace Host.ServerExceptions {
	public class SwitchingProtocolsFabric : ABSExceptionFabric {
		public SwitchingProtocolsFabric() {}

		public override string name {
			get {
				return "Switching Protocols";
			}
		}

        public override ExceptionCode Create(ExceptionUserCode userCode, object data)
        {
			return new SwitchingProtocols(userCode, data as string[]);
		}
	}
	public class SwitchingProtocols : ExceptionCode {
		private string[] protocols;

        public SwitchingProtocols(ExceptionUserCode userCode, params string[] protocols) : base(userCode)
        {
			this.protocols = protocols;
			Code = "101 Switching Protocols";
		}

        public override void ExceptionHandleCode(MIME.ABSMIME Handler, Reqest request, Response response, IConnetion handler)
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
		}
	}
}
