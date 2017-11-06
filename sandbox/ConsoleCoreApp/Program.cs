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
                //var postResult = client.Event.PostAsync(new DogEventPostParameter("TEST TITLE")
                //{
                //    Text = "AAA",
                //    AlertType = "Info",
                //    Tags = new[] { "test:1" },
                //    AggregationKey = "AAA",
                //}).Result;

                //postResult = client.Event.PostAsync(new DogEventPostParameter("TEST TITLE")
                //{
                //    Text = "AAA",
                //    AlertType = "Info",
                //    Tags = new[] { "test:1" },
                //    AggregationKey = "AAA",
                //}).Result;

                //var getResult = client.Event.GetAsync(postResult.Event.Id).Result;

                //var deleteResult = client.Event.DeleteAsync(getResult.Event.Id).Result;

                var result = client.Metric.GetListAsync(DateTimeOffset.Now - TimeSpan.FromDays(1)).Result;

            }
        }
    }
}
