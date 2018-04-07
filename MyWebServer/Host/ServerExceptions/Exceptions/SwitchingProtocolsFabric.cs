using Host.ConnectionHandlers;

namespace Host.ServerExceptions {
	public class SwitchingProtocolsFabric : ExceptionFabric {
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
			_IsFatal = true;
		}

		public override void ExceptionHandleCode(ref Reqest request, ref Response response, IConnetion handler) {
			string prot = "";
			for (int i = 0; i < protocols.Length; i++) {
				if (i == protocols.Length - 1) {
					prot += protocols[i];
				}
				else {
					prot += protocols[i] + ", ";
				}
			}
			response.AddToHeader("Upgrade", prot, AddMode.rewrite);
            response.AddToHeader("Connection", "Upgrade", AddMode.rewrite);
			response.AddForbiddenHeader("Content-Length");
			response.AddForbiddenHeader("Server");
		}
	}
}
