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
		public readonly IItem Resourse;

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
				try {
					Resourse = res;
					file_extension = res.Extension;
					try {
						data = res.GetData();
					}
					catch (NotImplementedException err) {
						data = null;
					}
				}
				catch (Exception err) {
					throw Repository.ExceptionFabrics["Internal Server Error"].Create(null, null);
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
