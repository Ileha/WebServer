using System;
using Host;
using Config;

namespace Host.HttpHandler {
    public interface IHttpHandler {
        string HandlerType { get; }

        void Parse(ref Reqest output, string[] reqest, string URI);
    }
}
