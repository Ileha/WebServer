using System;
using System.IO;
using Host.ServerExceptions;
using System.Text;
using Resouces;
using Host.Users;
using Host.ConnectionHandlers;

namespace Host
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
