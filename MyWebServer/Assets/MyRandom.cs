using System;

namespace Assets {
	public static class MyRandom {
		public static readonly Random rnd;

		static MyRandom() {
			rnd = new Random();
		}
	}
}
