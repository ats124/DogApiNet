using System;
using System.Runtime.Serialization;
using Utf8Json;
using DogApiNet;
using DogApiNet.Json;
using DogApiNet.JsonFormatters;
using System.Threading.Tasks;

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
                var postResult = client.Metric.PostAsync(new[]
                {
                    new DogMetric("test.metric", 500),
                    new DogMetric("test.metric2", 300)
                }).Result;

                var to = DateTimeOffset.Now;
                var from = to.AddMinutes(-30);
                var queryResult = client.Metric.QueryAsync(from, to, "test.metric{*}").Result;
            }
        }
    }
}
