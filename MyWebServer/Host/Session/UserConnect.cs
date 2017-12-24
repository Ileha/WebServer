using System;
using System.Collections.Generic;
using System.Threading;

namespace Host.Session {
	public class UserConnect
	{
		private static Dictionary<string, UserConnect> SessionInfo;
		private static long _long_live;
		private static Timer t;

		static UserConnect() {
			SessionInfo = new Dictionary<string, UserConnect>();
			_long_live = Convert.ToInt64(Repository.Configurate["session_collector"].Attribute("time_of_life").ToString());
			TimerCallback time = new TimerCallback(Collect);
			t = new Timer(time, null, 0, Convert.ToInt32(Repository.Configurate["session_collector"].Attribute("time_of_collect").ToString()));
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
			foreach (UserConnect usr in SessionInfo.Values) {
				
			}
		}


		public string ID { get; }
		private Dictionary<string, object> Data;
		private DateTime timeOfLife;

		public UserConnect()
		{
			ID = Guid.NewGuid().ToString("N");
			Data = new Dictionary<string, object>();
			SetDeletingTime();
			SessionInfo.Add(ID, this);
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
