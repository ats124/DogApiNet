using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Utf8Json;
using DogApiNet;
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
                var monitor = client.Monitor.CreateAsync(new DogMonitorCreateParameter(DogMonitorTypes.MericAlert, "max(last_1m):avg:test.random{*} > 90")
                {
                    Name = "metric alert testtest",
                    Message = "test alert message",
                    Options = new DogMonitorOptions()
                    {
                        NotifyNoData = true,
                        Locked = true,
                        NewHostDelay = 180,
                        RenotifyInterval = 30,
                        NoDataTimeframe = 3,
                        EscalationMessage = "hogehoge",
                        RequireFullWindow = false,
                        EvaluationDelay = 5,
                        Thresholds = new Dictionary<string, double>()
                        {
                            { "critical", 90 },
                            { "warning", 50 },
                            { "critical_recovery", 70 },
                            { "warning_recovery", 30 },
                        },
                        Silenced = new Dictionary<string, DateTimeOffset?>()
                        {
                            { "*", DateTimeOffset.Now.AddHours(5) }
                        }
                    }
                }).Result;

                var editMonitor = client.Monitor.EditAsync(monitor.Id, new DogMonitorEditParameter("max(last_1m):avg:test.random{*} > 80")
                {
                    Options = new DogMonitorOptions()
                    {
                        Thresholds = new Dictionary<string, double>()
                        {
                            { "critical", 80 },
                            { "warning", 40 },
                            { "critical_recovery", 60 },
                            { "warning_recovery", 20 },
                        },
                        Silenced = null,
                    }
                }).Result;

                var getMonitor = client.Monitor.GetAsync(monitor.Id, groupStates: "all").Result;

                var deleteMonitor = client.Monitor.DeleteAsync(monitor.Id).Result;

                var getAllMonitors = client.Monitor.GetAllAsync(groupStates: "all").Result;

                //var result = client.Monitor.ResolveAsync(new[] { new DogMonitorResolve(3505039, "*") }).Result;
            }
        }
    }
}
