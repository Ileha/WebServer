using System;
using System.IO;
using Host.ServerExceptions;
using Config;
using System.Text;

namespace Host
{
    public class Reader
    {
        public readonly byte[] data;
        public readonly string file_extension;

        public Reader(Reqest Reqest) {
            //string target = "";
            //try {
            //    target = Reqest.URL.Substring(1, Reqest.URL.Length - 1);
            //}
            //catch (Exception err) { throw new BadRequest(); }
            try {
				string path = Repository.Configurate._resourses.GetTargetRedirect(Reqest.URL);//Path.Combine(Repository.Configurate._resourses.GetTargetRedirect(target), target);
				//Console.WriteLine(path);
				if (File.Exists(path)) {
                    try {
                        file_extension = Path.GetExtension(path);
                        data = File.ReadAllBytes(path);
                    }
                    catch (Exception err) {
                        throw new InternalServerError();
                    }
                }
                else if (Repository.DirReader != null && Directory.Exists(path)) { //add check to null and working module
                    DirectoryInfo dir = new DirectoryInfo(path);
                    string str = "";
                    foreach (DirectoryInfo enemy in dir.GetDirectories()) {
                        str += Repository.DirReader.DirPars(enemy);
                    }
                    foreach (FileInfo enemy in dir.GetFiles()) {
                        str += Repository.DirReader.FilePars(enemy);
                    }
                    data = Encoding.UTF8.GetBytes(str);
                    file_extension = ".html";
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
