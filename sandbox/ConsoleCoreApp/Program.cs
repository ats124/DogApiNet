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

                //var result = client.Metric.GetListAsync(DateTimeOffset.Now - TimeSpan.FromDays(1)).Result;

                //var resutl = client.Metric.PostAsync(new DogMetricPostParameter("test.metric", DateTimeOffset.Now, 123.4)).Result;

                //var result = client.Metric.QueryAsync(DateTimeOffset.Now - TimeSpan.FromDays(1), DateTimeOffset.Now, "test.metric{*}by{host}").Result;

                //var updateResult = client.Metadata.UpdateAsync(new DogMetadataUpdateParameter("test.metric") { Description = null, ShortName = null, PerUnit = null, Unit = null, Type = null, StatsDInterval = null }).Result;
                //var getResutl = client.Metadata.GetAsync("test.metric").Result;
                var result = client.ServiceCheck.PostAsync(new DogServiceCheckPostParameter("app.is_ok", "test", 1)).Result;
                result = client.ServiceCheck.PostAsync(new DogServiceCheckPostParameter("app.is_ok_2", "test", 1) { Message = "message", Timestamp = DateTimeOffset.Now, Tags = new[] { "testtag:1" } }).Result;
            }
        }
    }
}
