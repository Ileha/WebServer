using System;
namespace Config
{
    public interface IConfigRead {
        string this[string index] { get; }
    }
}
