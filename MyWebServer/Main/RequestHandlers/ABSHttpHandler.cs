using System;
using System.Net.Sockets;
using System.IO;
using Configurate.Host.Connection.HTTPConnection;

namespace RequestHandlers {
    public abstract class ABSHttpHandler {
        public abstract string HandlerType { get; }
		public abstract string HandlerVersion { get; }
		private string _id = null;

        public abstract void ParseHeaders(ref Reqest output, Stream reqest);

		public string IDHandler() {
			if (_id == null) { _id = HandlerType + HandlerVersion; }
			return _id;
		}
    }
}
