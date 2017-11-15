using System;
using System.IO;
using Host.ServerExceptions;
using Config;

namespace Host
{
    public class Reader
    {
        public readonly byte[] data;
        public readonly string file_extension;

        public Reader(Reqest Reqest) {
            string path = Path.Combine(Repository.ReadConfig["root_dir"], Reqest.URL);
            if (!File.Exists(path)) {
                throw ExceptionCode.NotFound();
            }
            try {
                file_extension = Path.GetExtension(path);
                data = File.ReadAllBytes(path);
            }
            catch(Exception err) {
                throw ExceptionCode.InternalServerError();
            }
        }
    }
}
