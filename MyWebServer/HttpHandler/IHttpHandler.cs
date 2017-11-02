using System;
using MyWebServer;

namespace MyWebServer.HttpHandler {
    public abstract class IHttpHandler {
        public abstract TypeReqest HandlerType { get; }

        public abstract void Parse(ref Reqest output, string[] reqest, string URI);
    }
}
