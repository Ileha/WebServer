using System;
using MyWebServer;

namespace MyWebServer.HttpHandler {
    public abstract class IHttpHandler {
        public abstract TypeReqest HandlerType { get; }
        public abstract string Version { get; }
        public readonly string Identification;

        public IHttpHandler() {
            Identification = HttpHandlerIdentification(HandlerType, Version);
        }

        public abstract void Parse(ref Reqest output, string[] reqest, string URI);

        public static string HttpHandlerIdentification(TypeReqest type, string version) {
            return type.ToString() + version;
        }
    }
}
