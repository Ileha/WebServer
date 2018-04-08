using System;
using System.IO;
using Host.ServerExceptions;
using System.Text;
using Resouces;
using Host.Users;
using Host.ConnectionHandlers;

namespace Host
{
    public class Reader : IDisposable
    {
        public readonly Stream data;
        public readonly string file_extension;

        public Reader(string URL, UserInfo target_user) {
            try {
                IItem res = null;
                try {
                    res = Repository.Configurate.ResourceLinker.GetResourceByString(URL);
                }
                catch (FileNotFoundException err) {
					throw Repository.ExceptionFabrics["Not Found"].Create(null, null);
                }
                if (!res.IsUserEnter(target_user)) {
                    throw Repository.ExceptionFabrics["Unauthorized"].Create(null, "Access to staging site");
                }
                if (res.Extension == "dir") {
					if (Repository.DirReader == null) {
						throw Repository.ExceptionFabrics["Not Implemented"].Create(null, null);
					}
					StringBuilder str = new StringBuilder();
					str.Append(Repository.DirReader.ParsDirectoryHeader(res));
                    foreach (IItem ite in res) {
                        if (!ite.IsUserEnter(target_user)) { continue; }
						str.Append(Repository.DirReader.ItemPars(ite));
                    }
					str.Append(Repository.DirReader.ParsDirectoryDown(res));
					data = new MemoryStream();
					byte[] dt = Encoding.UTF8.GetBytes(str.ToString());
					data.Write(dt, 0, dt.Length);
					data.Seek(0, SeekOrigin.Begin);
                    file_extension = ".html";
                }
                else {
					try {
                        file_extension = res.Extension;
						data = res.GetData();
                    }
                    catch (Exception err) {
                        throw Repository.ExceptionFabrics["Internal Server Error"].Create(null, null);
                    }
                }
            }
            catch (Exception err) {
                if (err.GetType().IsSubclassOf(typeof(ExceptionCode))) {
                    throw err;
                }
                else {
                    throw Repository.ExceptionFabrics["Internal Server Error"].Create(null, null);
                }
            }
        }

		public void Dispose() {
			data.Dispose();
		}
	}
}
