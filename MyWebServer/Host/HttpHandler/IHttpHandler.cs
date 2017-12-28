using System;
using Host;
using Config;
using System.Net.Sockets;

namespace Host.HttpHandler {
    public abstract class IHttpHandler {
        public abstract string HandlerType { get; }
		public abstract string HandlerVersion { get; }
		private string _id;

		public IHttpHandler() {
			_id = HandlerType + HandlerVersion;
		}

        public abstract void ParseHeaders(ref Reqest output, string[] reqest, string URI);
		public abstract void ParseData(ref Reqest output, TcpClient data_sourse);
		public string IDHandler() {
			return _id;
		}
    }
}
