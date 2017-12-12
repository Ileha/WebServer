using System;
using System.IO;
using Host.ServerExceptions;
using Config;
using System.Text;
using Resouces;

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
                IItem res = Repository.Configurate.ResourceLinker.GetResourceByString(Reqest.URL);
                //string path = res.GetInfo().FullName;//Path.Combine(Repository.Configurate._resourses.GetTargetRedirect(target), target);
				//Console.WriteLine(path);
                if (res.GetType() == typeof(LinkFile)) {
                    try {
                        file_extension = Path.GetExtension(res.GetInfo().Extension);
                        data = System.IO.File.ReadAllBytes(res.GetInfo().FullName);
                    }
                    catch (Exception err) {
                        throw new InternalServerError();
                    }
                }
                else if (Repository.DirReader != null && res.GetType() == typeof(LinkDirectory)) {
                    //DirectoryInfo dir = new DirectoryInfo(path);
                    string str = "";
                    foreach (IItem ite in res) {
                        str += Repository.DirReader.ItemPars(ite);
                    }
                    //foreach (DirectoryInfo enemy in dir.GetDirectories()) {
                    //    str += Repository.DirReader.DirPars(enemy);
                    //}
                    //foreach (FileInfo enemy in dir.GetFiles()) {
                    //    str += Repository.DirReader.FilePars(enemy);
                    //}
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
