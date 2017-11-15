using System;
using Host;
using Config;

namespace Host.HttpHandler {
    public interface IHttpHandler {
        TypeReqest HandlerType { get; }

        void Parse(ref Reqest output, string[] reqest, string URI);

		void Info();
    }
}
