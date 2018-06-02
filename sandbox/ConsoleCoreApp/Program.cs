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
                Debug.Assert(downtime.Scope[0] == param.Scope[0]);
                Debug.Assert(downtime.Start.Value.Normalize() == param.Start.Value.Normalize());
                Debug.Assert(downtime.End.Value.Normalize()== param.End.Value.Normalize());
                Debug.Assert(downtime.Message == param.Message);
                Debug.Assert(downtime.MonitorId == param.MonitorId);
                Debug.Assert(downtime.Recurrence.Type == param.Recurrence.Type);
                Debug.Assert(downtime.Recurrence.Period == param.Recurrence.Period);
                Debug.Assert(downtime.Recurrence.WeekDays[0] == param.Recurrence.WeekDays[0]);
                Debug.Assert(downtime.Recurrence.WeekDays[1] == param.Recurrence.WeekDays[1]);
                Debug.Assert(downtime.Recurrence.UntilOccurrences == param.Recurrence.UntilOccurrences);
                Debug.Assert(downtime.TimeZone == param.TimeZone);

                var updateParam = new DogDowntimeUpdateParameter(downtime.Id, downtime.Scope)
                {
                    Start = start.AddDays(1),
                    End = start.AddDays(2),
                    Message = "test message edit",
                    MonitorId = null,
                    Recurrence = new DogDowntimeRecurrence(DogDowntimeRecurrenceTypes.Weeks, 2)
                    {
                        WeekDays = new [] { DayOfWeek.Wednesday },
                        UntilDate = start.AddDays(5),
                    },
                    TimeZone = "Asia/Japan",
                };
                var updateDowntime = await client.Downtime.UpdateAsync(updateParam);
                Debug.Assert(updateDowntime.Scope[0] == updateParam.Scope[0]);
                Debug.Assert(updateDowntime.Start.Value.Normalize() == updateParam.Start.Value.Normalize());
                Debug.Assert(updateDowntime.End.Value.Normalize() == updateParam.End.Value.Normalize());
                Debug.Assert(updateDowntime.Message == updateParam.Message);
                Debug.Assert(updateDowntime.MonitorId == updateParam.MonitorId);
                Debug.Assert(updateDowntime.Recurrence.Type == updateParam.Recurrence.Type);
                Debug.Assert(updateDowntime.Recurrence.Period == updateParam.Recurrence.Period);
                Debug.Assert(updateDowntime.Recurrence.WeekDays[0] == updateParam.Recurrence.WeekDays[0]);
                Debug.Assert(updateDowntime.Recurrence.UntilOccurrences == updateParam.Recurrence.UntilOccurrences);
                Debug.Assert(updateDowntime.TimeZone == updateParam.TimeZone);

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