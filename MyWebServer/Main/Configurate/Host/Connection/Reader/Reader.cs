using ExceptionFabric;
using Configurate.Users;
using Configurate.Resouces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Host.Connection.Reader
{
    public class Reader
    {
        private string _url;
        private Stream _data;
        private IitemRead _resourse;

        public Reader(Reader origin) {
            _url = origin._url;
            _resourse = origin._resourse;
            _data = _resourse.GetData();
        }

        public Reader(string URL, UserInfo target_user) {
            try {
                try {
                    _resourse = Repository.Configurate.ResourceLinker.GetResourceByString(URL);
                    _url = URL;
                }
                catch (FileNotFoundException err) {
                    throw Repository.ExceptionFabrics["Not Found"].Create();
                }
                if (!_resourse.IsUserEnter(target_user))
                {
                    throw Repository.ExceptionFabrics["Unauthorized"].Create("Access to staging site");
                }
                try {
                    try {
                        _data = _resourse.GetData();
                    }
                    catch (NotImplementedException err) {
                        _data = null;
                    }
                }
                catch (Exception err) {
                    throw Repository.ExceptionFabrics["Internal Server Error"].Create();
                }
            }
            catch (ExceptionCode err) {
                throw err;
            } 
            catch (Exception err) {
                throw Repository.ExceptionFabrics["Internal Server Error"].Create();
            }
        }

        public void Dispose()
        {
            if (_data != null) {
                _data.Dispose();
            }
        }

        public Stream Data {
            get {
                return _data; 
            }
        }

        public string FileExtension
        {
            get { return _resourse.Extension; }
        }

        public IItem Resourse
        {
            get { return _resourse; }
        }

        public string URL
        {
            get { return _url; }
        }
    }
}
