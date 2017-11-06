using System;
using System.Collections.Generic;
using System.Text;

namespace DogApiNet
{
    static internal class DogApiUtil
    {
        static readonly DateTimeOffset _unixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static long ToUnixTimeSeconds(this DateTimeOffset @this)
            => (long)(@this - _unixEpoch).TotalSeconds;

        public static long ToUnixTimeMilliseconds(this DateTimeOffset @this)
            => (long)(@this - _unixEpoch).TotalMilliseconds;

        public static DateTimeOffset UnixTimeSecondsToDateTimeOffset(long seconds)
            => _unixEpoch.AddSeconds(seconds);

        public static DateTimeOffset UnixTimeMillisecondsToDateTimeOffset(long milliseconds)
            => _unixEpoch.AddMilliseconds(milliseconds);
    }
}
