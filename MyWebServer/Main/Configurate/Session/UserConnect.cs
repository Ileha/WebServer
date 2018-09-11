using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using Configurate.Session.Exception;

namespace Configurate.Session {
	public class UserConnect {
        public static ConcurrentDictionary<string, UserConnect> SessionInfo { get; private set; }

		static UserConnect() {
            SessionInfo = new ConcurrentDictionary<string, UserConnect>();
		}

		public static UserConnect GetUserDataFromID(string id) {
			try {
				UserConnect session = SessionInfo[id];
				session.SetDeletingTime();
				return session;
			}
			catch (KeyNotFoundException err) {
				throw new UserNotFound(id);
			}
		}

        public static void Dispose() {
            SessionInfo.Clear();
        }

        private string _id;
        public string ID {
            get { return _id; }
        }
		private Dictionary<string, object> Data;
		private DateTime timeOfLife;
		public DateTime TimeOfLife {
			get { return timeOfLife; }
		}

		public UserConnect()
		{			
			Data = new Dictionary<string, object>();
			SetDeletingTime();
			bool is_exists = false;
			do {
				_id = Guid.NewGuid().ToString("N");
				is_exists = SessionInfo.TryAdd(ID, this);
			} while (is_exists == false);
		}
		private void SetDeletingTime() {
			timeOfLife = new DateTime(DateTime.Now.Ticks);
		}

		public void Clear() {
			Data.Clear();
		}

		public void RemoveData(string name) {
			Data.Remove(name);
		}

		public void AddData(string name, object data) {
			try {
				Data.Add(name, data);
			}
			catch (System.Exception err) {
				Data[name] = data;
			}
		}

		public T GetData<T>(string name) where T : class {
			SetDeletingTime();
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
		//public static bool operator ==(UserConnect obj1, UserConnect obj2) {
		//	return obj1.ID == obj2.ID;
		//}
		//public static bool operator !=(UserConnect obj1, UserConnect obj2)
		//{
		//	return obj1.ID != obj2.ID;
		//}
	}
}
