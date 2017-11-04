using System;
using MyWebServer;
using MyWebServer.WebServerConfigure;

namespace MyWebServer.HttpHandler {
    public abstract class IHttpHandler {
        public abstract TypeReqest HandlerType { get; }

        public abstract void Parse(ref Reqest output, string[] reqest, string URI, IConfigRead redirectTable);
    }
}
