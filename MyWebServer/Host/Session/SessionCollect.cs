using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Config;

namespace Host.Session
{
	public class SessionCollect : IConfigurate
	{
		private long time_of_live;
		private Timer t;

		public SessionCollect() {
			
		}

		public string ConfigName {
			get { return "session_collector"; }
		}

		public void Configurate(XElement data) {
			time_of_live = Convert.ToInt64(data.Attribute("time_of_life").Value.ToString());
			TimerCallback time = new TimerCallback(Collect);
			t = new Timer(time, null, 0, Convert.ToInt32(data.Attribute("time_of_collect").Value.ToString()));
		}

		private void Collect(object obj) {
			DateTime now_date = DateTime.Now.Subtract(new TimeSpan(10000*time_of_live));
			UserConnect[] res = UserConnect.SessionInfo.Select(x => x.Value).ToArray();
			UserConnect out_rem;
			foreach (UserConnect s in res) {
				if (s.TimeOfLife.CompareTo(now_date) == -1) {
					UserConnect.SessionInfo.TryRemove(s.ID, out out_rem);
				}
			}
			Console.WriteLine(string.Format("{0} tick !!!\r\nCollect count {1}", Repository.Configurate["name"].Value, UserConnect.SessionInfo.Count));
		}
	}
}
