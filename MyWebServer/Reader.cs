using System;
using System.IO;
using MyWebServer.ServerExceptions;

namespace MyWebServer
{
    public class Reader
    {
        public readonly byte[] data;
        public readonly string file_extension;

        public Reader(Reqest Reqest, string work_dir) {
            string path = Path.Combine(work_dir, Reqest.URL);
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
