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
                var monitor = await client.Monitor.CreateAsync(new DogMonitorCreateParameter(DogMonitorTypes.MericAlert, "max(last_1m):avg:test.random{*} > 80"));

                var start = DateTimeOffset.UtcNow.AddDays(1);
                var param = new DogDowntimeCreateParameter("*")
                {
                    Start = start,
                    End = start.AddDays(1),
                    Message = "test message",
                    MonitorId = monitor.Id,
                    Recurrence = new DogDowntimeRecurrence(DogDowntimeRecurrenceTypes.Days, 1)
                    {
                        WeekDays = new [] { DayOfWeek.Monday, DayOfWeek.Saturday },
                        //UntilDate = start.AddDays(10),
                        UntilOccurrences = 5,
                    },
                    TimeZone = "UTC"
                };
                var downtime = await client.Downtime.CreateAsync(param);
                await client.Downtime.DeleteAsync(downtime.Id);

                await client.Monitor.DeleteAsync(monitor.Id);
            }
        }

        private static DateTimeOffset Normalize(this DateTimeOffset @this)
        {
            @this = @this.UtcDateTime;
            return new DateTimeOffset(@this.Year, @this.Month, @this.Day, @this.Hour, @this.Minute, @this.Second, TimeSpan.Zero);
        }
    }
}