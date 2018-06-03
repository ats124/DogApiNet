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
                var monitor = await client.Monitor.CreateAsync(new DogMonitor(DogMonitorTypes.MericAlert, "max(last_1m):avg:test.random{*} > 80"));

                var start = DateTimeOffset.UtcNow.AddDays(1);
                var downtime = await client.Downtime.CreateAsync(new DogDowntime("*")
                {
                    Start = start,
                    End = start.AddDays(1),
                    Message = "test message",
                    MonitorId = monitor.Id,
                    Recurrence = new DogDowntimeRecurrence(DogDowntimeRecurrenceTypes.Days, 1)
                    {
                        WeekDays = new[] {DayOfWeek.Monday, DayOfWeek.Saturday},
                        //UntilDate = start.AddDays(10),
                        UntilOccurrences = 5,
                    },
                    TimeZone = "UTC"
                });

                downtime.Start = downtime.Start.Value.AddDays(1);
                downtime.End = downtime.Start.Value.AddDays(1);
                downtime.Message = "test message edit";
                downtime.MonitorId = null;
                downtime.Recurrence.WeekDays = new[] {DayOfWeek.Wednesday};
                downtime.Recurrence.UntilDate = start.AddDays(5);
                downtime.TimeZone = "Asia/Japan";

                var getDowntime = await client.Downtime.GetAsync(downtime.Id);

                var updateDowntime = await client.Downtime.UpdateAsync(downtime);
                Debug.Assert(updateDowntime.Scope[0] == downtime.Scope[0]);
                Debug.Assert(updateDowntime.Start.Value.Normalize() == downtime.Start.Value.Normalize());
                Debug.Assert(updateDowntime.End.Value.Normalize() == downtime.End.Value.Normalize());
                Debug.Assert(updateDowntime.Message == downtime.Message);
                Debug.Assert(updateDowntime.MonitorId == downtime.MonitorId);
                Debug.Assert(updateDowntime.Recurrence.Type == downtime.Recurrence.Type);
                Debug.Assert(updateDowntime.Recurrence.Period == downtime.Recurrence.Period);
                Debug.Assert(updateDowntime.Recurrence.WeekDays[0] == downtime.Recurrence.WeekDays[0]);
                Debug.Assert(updateDowntime.Recurrence.UntilOccurrences == downtime.Recurrence.UntilOccurrences);
                Debug.Assert(updateDowntime.TimeZone == downtime.TimeZone);

                var getAllDowntime = await client.Downtime.GetAllAsync();
                foreach (var d in getAllDowntime)
                {
                    await client.Downtime.DeleteAsync(d.Id);
                }

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