using System;
namespace MyWebServer.WebServerConfigure
{
    public interface IConfigRead {
        string this[string index] { get; }
    }
}
