using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Utf8Json;
using DogApiNet;
using DogApiNet.JsonFormatters;
using System.Threading.Tasks;

namespace ConsoleCoreApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("DATADOG_API_KEY");
            var appKey = Environment.GetEnvironmentVariable("DATADOG_APP_KEY");

            using (var client = new DogApiClient(apiKey, appKey))
            {
                var all = await client.Embed.GetAllAsync();
                var embed = await client.Embed.CreateAsync(new DogEmbedCreateParameter("{\r\n  \"viz\": \"timeseries\",\r\n  \"status\": \"done\",\r\n  \"requests\": [\r\n    {\r\n      \"q\": \"avg:test.random{*}\",\r\n      \"type\": \"line\",\r\n      \"style\": {\r\n        \"palette\": \"dog_classic\",\r\n        \"type\": \"solid\",\r\n        \"width\": \"normal\"\r\n      },\r\n      \"conditional_formats\": [],\r\n      \"aggregator\": \"avg\"\r\n    }\r\n  ],\r\n  \"autoscale\": true,\r\n  \"events\": [\r\n    {\r\n      \"q\": \"test.random \",\r\n      \"tags_execution\": \"and\"\r\n    }\r\n  ]\r\n}")
                {
                    Title = "hoge",
                    Legend = true,
                    Timeframe = "1_hour",
                    Size = "large",
                });
            }
        }

        private static DateTimeOffset Normalize(this DateTimeOffset @this)
        {
            @this = @this.UtcDateTime;
            return new DateTimeOffset(@this.Year, @this.Month, @this.Day, @this.Hour, @this.Minute, @this.Second, TimeSpan.Zero);
        }
    }
}