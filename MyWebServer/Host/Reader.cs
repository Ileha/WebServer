using System;
using System.IO;
using Host.ServerExceptions;
using Config;
using System.Text;
using Resouces;
using Host.Users;

namespace Host
{
    public class Reader
    {
        public readonly byte[] data;
        public readonly string file_extension;

        public Reader(Reqest Reqest, UserInfo target_user) {
            try {
                IItem res = null;
                try {
                    res = Repository.Configurate.ResourceLinker.GetResourceByString(Reqest.URL);
                }
                catch (FileNotFoundException err) {
					throw Repository.ExceptionFabrics["Not Found"].Create(null);
                }
                if (!res.IsUserEnter(target_user)) {
                    throw Repository.ExceptionFabrics["Unauthorized"].Create("Access to staging site");
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
                else if (Repository.DirReader != null && res.GetType() == typeof(LinkDirectory)) {
					string str = Repository.DirReader.ParsDirectoryHeader(res);
                    foreach (IItem ite in res) {
                        if (!ite.IsUserEnter(target_user)) { continue; }
                        str += Repository.DirReader.ItemPars(ite);
                    }
					str += Repository.DirReader.ParsDirectoryDown(res);
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
