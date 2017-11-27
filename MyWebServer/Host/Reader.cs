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
            string target = Reqest.URL.Substring(1, Reqest.URL.Length - 1);
            string path = Path.Combine(Repository.ReadConfig["root_dir"], target);
            if (!File.Exists(path)) {
                throw new NotFound();
            }
            try {
                file_extension = Path.GetExtension(path);
                data = File.ReadAllBytes(path);
            }
            catch(Exception err) {
                throw new InternalServerError();
            }
        }
    }
}
