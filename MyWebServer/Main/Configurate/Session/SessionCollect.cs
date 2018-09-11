using System;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Configurate.Session
{
	public class SessionCollect : IConfigurate, IDisposable
	{
		private long time_of_live;
		private Timer t;

		public SessionCollect() {
			
		}

        private string[] names = new string[] { "session_collector" };

		public string[] ConfigName {
            get { return names; }
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
			//Console.WriteLine(string.Format("{0} tick !!!\r\nCollect count {1}", Repository.Configurate["name"].Value, UserConnect.SessionInfo.Count));
		}

        public void Dispose() {
            t.Change(Timeout.Infinite, Timeout.Infinite);
            t.Dispose();
            UserConnect.Dispose();
        }
    }
}
