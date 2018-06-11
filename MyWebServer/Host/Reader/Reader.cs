using Host;
using Host.ServerExceptions;
using Host.Users;
using Resouces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host.ConnectionHandlers
{
    public class Reader : IReader
    {

        private string _url;
        private Stream _data;
        private IItem _resourse;

        public Reader(string URL, UserInfo target_user) {
            try
            {
                IItem res = null;
                try
                {
                    res = Repository.Configurate.ResourceLinker.GetResourceByString(URL);
                    _url = URL;
                }
                catch (FileNotFoundException err)
                {
                    throw Repository.ExceptionFabrics["Not Found"].Create(null, null);
                }
                if (!res.IsUserEnter(target_user))
                {
                    throw Repository.ExceptionFabrics["Unauthorized"].Create(null, "Access to staging site");
                }
                try
                {
                    _resourse = res;
                    try
                    {
                        _data = res.GetData();
                    }
                    catch (NotImplementedException err)
                    {
                        _data = null;
                    }
                }
                catch (Exception err)
                {
                    throw Repository.ExceptionFabrics["Internal Server Error"].Create(null, null);
                }
            }
            catch (ExceptionCode err) {
                throw err;
            } 
            catch (Exception err) {
                throw Repository.ExceptionFabrics["Internal Server Error"].Create(null, null);
            }
        }

        public void Dispose()
        {
            if (_data != null) {
                _data.Dispose();
            }
        }

        public Stream Data
        {
            get { return _data; }
        }

        public string FileExtension
        {
            get { return _resourse.Extension; }
        }

        public IItem Resourse
        {
            get { return _resourse; }
        }

        public void UpdateData()
        {
            Dispose();
            if (_resourse != null) {
                _data = _resourse.GetData();
            }
        }


        public string URL
        {
            get { return _url; }
        }
    }
}
