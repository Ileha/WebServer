using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host.Session;

namespace Host.Session
{
    class ConnectSession
    {
        private string _id;
        public string ID {
            get { return _id; }
        }
		private Dictionary<string, object> Data;
		private DateTime timeOfLife;
        public DateTime TimeOfLife {
            get { return timeOfLife; }
        }

        public ConnectSession()
		{			
			Data = new Dictionary<string, object>();
			SetDeletingTime();
			bool is_exists = false;
			do {
				_id = Guid.NewGuid().ToString("N");
				is_exists = UserConnect.SessionInfo.TryAdd(ID, this);
			} while (is_exists == false);
		}
		public void SetDeletingTime() {
			timeOfLife = new DateTime(DateTime.Now.Ticks+10000*UserConnect.LongLife);
		}

		public T GetData<T>(string name) where T : class {
			try {
				object data = Data[name];
				if (data is T) {
					return data as T;
				}
				else {
					throw new DataTypeException(data.GetType(), typeof(T));
				}
			}
			catch (KeyNotFoundException err) {
				throw new UserDataNotFound(name);
			}
		}
    }
}
