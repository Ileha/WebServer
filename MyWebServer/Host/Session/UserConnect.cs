using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;

namespace Host.Session {
	public class UserConnect
	{
		private static ConcurrentDictionary<string, UserConnect> SessionInfo;
		private static long _long_live;
		private static Timer t;

		static UserConnect() {
			SessionInfo = new ConcurrentDictionary<string, UserConnect>();
			_long_live = Convert.ToInt64(Repository.Configurate["session_collector"].Attribute("time_of_life").Value.ToString());
			TimerCallback time = new TimerCallback(Collect);
			t = new Timer(time, null, 0, Convert.ToInt32(Repository.Configurate["session_collector"].Attribute("time_of_collect").Value.ToString()));
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

		private static void Collect(object obj) {
			UserConnect[] res = SessionInfo.Select(x => x.Value).ToArray();
			UserConnect out_rem;
			foreach (UserConnect s in res) {
				if (s.timeOfLife.CompareTo(DateTime.Now) == -1) {
					SessionInfo.TryRemove(s.ID, out out_rem);
				}
			}
			Console.WriteLine(string.Format("{0} tick !!!\r\nCollect count {1}", Repository.Configurate["name"].Value, SessionInfo.Count));
		}

        private string _id;
        public string ID {
            get { return _id; }
        }
		private Dictionary<string, object> Data;
		private DateTime timeOfLife;

		public UserConnect()
		{			
			Data = new Dictionary<string, object>();
			SetDeletingTime();
			bool is_exists = false;
			do {
				_id = Guid.NewGuid().ToString("N");
				is_exists = SessionInfo.TryAdd(ID, this);
			} while (is_exists == false);
			//deleting_queue.Add(this);
		}
		private void SetDeletingTime() {
			timeOfLife = new DateTime(DateTime.Now.Ticks+10000*_long_live);
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
		//public static bool operator ==(UserConnect obj1, UserConnect obj2) {
		//	return obj1.ID == obj2.ID;
		//}
		//public static bool operator !=(UserConnect obj1, UserConnect obj2)
		//{
		//	return obj1.ID != obj2.ID;
		//}
	}
}
