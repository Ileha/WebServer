using System;
using System.IO;
using Configurate.Resouces;

namespace Configurate.Host.Connection.Reader
{
    public interface IReader : IDisposable
    {
        string URL { get; }
        Stream Data { get; }
        string FileExtension { get; }
        IItem Resourse { get; }
        void UpdateData();
	}
}
