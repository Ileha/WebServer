using System;
using Host;
using Config;

namespace Host.HttpHandler {
    public abstract class IHttpHandler {
        public abstract string HandlerType { get; }
		public abstract string HandlerVersion { get; }
		private string _id;

		public IHttpHandler() {
			_id = HandlerType + HandlerVersion;
		}

        public abstract void Parse(ref Reqest output, string[] reqest, string URI);
		public string IDHandler() {
			return _id;
		}
    }
}
