using System;
using DogApiNet;

namespace ConsoleCoreApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("DATADOG_API_KEY");
            var appKey = Environment.GetEnvironmentVariable("DATADOG_APP_KEY");

            using (var client = new DogApiClient(apiKey, appKey))
            {
                var end = DateTimeOffset.Now;
                var start = end.AddMinutes(-10);

                var events = client.Event.QueryAsync(start, end).Result;
            }
        }
    }
}
