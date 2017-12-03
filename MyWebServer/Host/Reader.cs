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
            try {
                if (File.Exists(path)) {
                    try {
                        file_extension = Path.GetExtension(path);
                        data = File.ReadAllBytes(path);
                    }
                    catch (Exception err) {
                        throw new InternalServerError();
                    }
                }
                else if (Directory.Exists(path)) {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    foreach (DirectoryInfo enemy in dir.GetDirectories()) {
                        Repository.DirReader.DirPars(enemy);
                    }
                    foreach (FileInfo enemy in dir.GetFiles())
                    {
                        Repository.DirReader.FilePars(enemy);
                    }
                }
                else {
                    throw new NotFound();
                }
            }
            catch (Exception err) {
                if (err.GetType().IsSubclassOf(typeof(ExceptionCode))) {
                    throw err;
                }
                else {
                    throw new InternalServerError();
                }
            }
        }
    }
}
