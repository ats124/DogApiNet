using System;

namespace DogApiNet
{
    internal static class DogApiUtil
    {
        private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public static long ToUnixTimeSeconds(this DateTimeOffset @this)
            => (long)(@this - UnixEpoch).TotalSeconds;

        public static long ToUnixTimeMilliseconds(this DateTimeOffset @this)
            => (long)(@this - UnixEpoch).TotalMilliseconds;

        public static DateTimeOffset UnixTimeSecondsToDateTimeOffset(long seconds)
            => UnixEpoch.AddSeconds(seconds);

        public static DateTimeOffset UnixTimeMillisecondsToDateTimeOffset(long milliseconds)
            => UnixEpoch.AddMilliseconds(milliseconds);
    }
}