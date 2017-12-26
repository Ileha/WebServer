using System;
using System.Collections.Generic;
using System.Threading;

namespace Host.Session {
	public class UserConnect
	{
		private static Dictionary<string, UserConnect> SessionInfo;
        private static Queue<UserConnect> deleting_queue;
		private static long _long_live;
		private static Timer t;

		static UserConnect() {
			SessionInfo = new Dictionary<string, UserConnect>();
			_long_live = Convert.ToInt64(Repository.Configurate["session_collector"].Attribute("time_of_life").Value.ToString());
			TimerCallback time = new TimerCallback(Collect);
			t = new Timer(time, null, 0, Convert.ToInt32(Repository.Configurate["session_collector"].Attribute("time_of_collect").Value.ToString()));
            deleting_queue = new Queue<UserConnect>();
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
            if (deleting_queue.Count == 0) { return; }
            UserConnect curr_delete = deleting_queue.Peek();
            while (curr_delete.timeOfLife.CompareTo(DateTime.Now) == -1) {
                SessionInfo.Remove(curr_delete.ID);
                deleting_queue.Dequeue();
                if (deleting_queue.Count == 0) { break; }
                curr_delete = deleting_queue.Peek();
            }
			//Console.WriteLine(string.Format("{0} tick !!!\r\nCollect count {1}\r\nQueue count {2}", Repository.Configurate["name"].Value, SessionInfo.Count, deleting_queue.Count));
		}

        private string _id;
        public string ID {
            get { return _id; }
        }
		private Dictionary<string, object> Data;
		private DateTime timeOfLife;

		public UserConnect()
		{
			_id = Guid.NewGuid().ToString("N");
			Data = new Dictionary<string, object>();
			SetDeletingTime();
			SessionInfo.Add(ID, this);
            deleting_queue.Enqueue(this);
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
	}
}
