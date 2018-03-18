namespace Host.ServerExceptions {
	public class SwitchingProtocolsFabric : ExceptionFabric {
		public SwitchingProtocolsFabric() {}

		public override string name {
			get {
				return "Switching Protocols";
			}
		}

		public override ExceptionCode Create(object data) {
			return new SwitchingProtocols(data as string[]);
		}
	}
	public class SwitchingProtocols : ExceptionCode {
		private string[] protocols;

		public SwitchingProtocols(params string[] protocols) {
			this.protocols = protocols;
			Code = "101 Switching Protocols";
			_IsFatal = true;
		}

		public override void ExceptionHandle(ref Reqest request, ref Response response) {
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
		}
	}
}
