using System;
using System.Threading;

namespace Assets
{
    public static class MyRandom
    {
        private static readonly Random main_random;
        private static Random Rand
        {
            get
            {
                lock (global_lock)
                {
                    return new Random(main_random.Next());
                }
            }
        }
        private static object global_lock;
        private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(() =>
        {
            return Rand;
        });
        public static Random rnd { get { return threadRandom.Value; } }

        static MyRandom()
        {
            main_random = new Random();
            global_lock = new object();
        }

        public static double GetRandomDouble()
        {
            return rnd.NextDouble();
        }
        public static double GetRandomDouble(double max)
        {
            return GetRandomDouble() % max;
        }
        public static double GetRandomDouble(double min, double max)
        {
            return GetRandomDouble() * (max - min) + min;
        }
    }
}
