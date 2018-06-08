using System;
using System.Collections.Generic;
using System.Text;

namespace DogApiNet
{
    public class DogApiRateLimit
    {
        public int Limit { get; }
        public int Period { get; }
        public int Remainig { get; }
        public int Reset { get; }

        public DogApiRateLimit(int limit, int period, int remaining, int reset)
        {
            Limit = limit;
            Period = period;
            Remainig = remaining;
            Reset = reset;
        }
    }
}
