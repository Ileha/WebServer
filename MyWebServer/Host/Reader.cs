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
            try {
                IItem res = null;
                try {
                    res = Repository.Configurate.ResourceLinker.GetResourceByString(Reqest.URL);
                }
                catch (FileNotFoundException err) {
					throw Repository.ExceptionFabrics["Not Found"].Create(null);
                }
                if (res.GetType() == typeof(LinkFile)) {
                    try {
                        file_extension = Path.GetExtension(res.GetInfo().Extension);
                        data = System.IO.File.ReadAllBytes(res.GetInfo().FullName);
                    }
                    catch (Exception err) {
                        throw Repository.ExceptionFabrics["Internal Server Error"].Create(null);
                    }
                }
                else if (Repository.Configurate.DirReader != null && res.GetType() == typeof(LinkDirectory)) {
                    string str = "";
                    foreach (IItem ite in res) {
                        str += Repository.Configurate.DirReader.ItemPars(ite);
                    }
                    data = Encoding.UTF8.GetBytes(str);
                    file_extension = ".html";
                }
                else {
                    throw Repository.ExceptionFabrics["Not Found"].Create(null);
                }
            }
            catch (Exception err) {
                if (err.GetType().IsSubclassOf(typeof(ExceptionCode))) {
                    throw err;
                }
                else {
                    throw Repository.ExceptionFabrics["Internal Server Error"].Create(null);
                }
            }
        }
    }
}
