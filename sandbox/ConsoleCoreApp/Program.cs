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
                var tags = new[] { "testtag1", "testtag2" };
                var silenced = DateTimeOffset.Now.AddHours(5);
                var monitor = await client.Monitor.CreateAsync(
                    new DogMonitorCreateParameter(DogMonitorTypes.MericAlert, "max(last_1m):avg:test.random{*} > 90")
                    {
                        Name = "metric alert testtest",
                        Message = "test alert message",
                        Tags = tags,
                        Options = new DogMonitorOptions()
                        {
                            IncludeTags = true,
                            TimeoutHours = 2,
                            NotifyAudit = true,
                            NotifyNoData = true,
                            Locked = true,
                            NewHostDelay = 180,
                            RenotifyInterval = 30,
                            NoDataTimeframe = 3,
                            EscalationMessage = "hogehoge",
                            RequireFullWindow = true,
                            EvaluationDelay = 5,
                            Thresholds = new Dictionary<string, double>()
                            {
                                {"critical", 90},
                                {"warning", 50},
                                {"critical_recovery", 70},
                                {"warning_recovery", 30},
                            },
                            Silenced = new Dictionary<string, DateTimeOffset?>()
                            {
                                {"*", silenced}
                            }
                        }
                    });
                Debug.Assert(monitor.Query == "max(last_1m):avg:test.random{*} > 90");
                Debug.Assert(monitor.Name == "metric alert testtest");
                Debug.Assert(monitor.Message == "test alert message");
                Debug.Assert(monitor.Tags[0] == tags[0]);
                Debug.Assert(monitor.Tags[1] == tags[1]);
                Debug.Assert(monitor.Options.IncludeTags);
                Debug.Assert(monitor.Options.TimeoutHours == 2);
                Debug.Assert(monitor.Options.NotifyAudit);
                Debug.Assert(monitor.Options.NotifyNoData);
                Debug.Assert(monitor.Options.Locked);
                Debug.Assert(monitor.Options.NewHostDelay == 180);
                Debug.Assert(monitor.Options.RenotifyInterval == 30);
                Debug.Assert(monitor.Options.NoDataTimeframe == 3);
                Debug.Assert(monitor.Options.EscalationMessage == "hogehoge");
                Debug.Assert(monitor.Options.RequireFullWindow);
                Debug.Assert(monitor.Options.Thresholds["critical"] == 90.0);
                Debug.Assert(monitor.Options.Thresholds["warning"] == 50);
                Debug.Assert(monitor.Options.Thresholds["critical_recovery"] == 70);
                Debug.Assert(monitor.Options.Thresholds["warning_recovery"] == 30);
                Debug.Assert(monitor.Options.Silenced["*"].Value.Normalize() ==  silenced.Normalize());

                var getMonitor = await client.Monitor.GetAsync(monitor.Id, groupStates: "all");
                Debug.Assert(getMonitor.Query == "max(last_1m):avg:test.random{*} > 90");
                Debug.Assert(getMonitor.Name == "metric alert testtest");
                Debug.Assert(getMonitor.Message == "test alert message");
                Debug.Assert(getMonitor.Tags[0] == tags[0]);
                Debug.Assert(getMonitor.Tags[1] == tags[1]);
                Debug.Assert(getMonitor.Options.IncludeTags);
                Debug.Assert(getMonitor.Options.TimeoutHours == 2);
                Debug.Assert(getMonitor.Options.NotifyAudit);
                Debug.Assert(getMonitor.Options.NotifyNoData);
                Debug.Assert(getMonitor.Options.Locked);
                Debug.Assert(getMonitor.Options.NewHostDelay == 180);
                Debug.Assert(getMonitor.Options.RenotifyInterval == 30);
                Debug.Assert(getMonitor.Options.NoDataTimeframe == 3);
                Debug.Assert(getMonitor.Options.EscalationMessage == "hogehoge");
                Debug.Assert(getMonitor.Options.RequireFullWindow);
                Debug.Assert(getMonitor.Options.Thresholds["critical"] == 90);
                Debug.Assert(getMonitor.Options.Thresholds["warning"] == 50);
                Debug.Assert(getMonitor.Options.Thresholds["critical_recovery"] == 70);
                Debug.Assert(getMonitor.Options.Thresholds["warning_recovery"] == 30);
                Debug.Assert(getMonitor.Options.Silenced["*"].Value.Normalize() == silenced.Normalize());

                var tags2 = new[] { "testtag3" };
                var silenced2 = DateTimeOffset.Now.AddHours(6);
                var editMonitor = await client.Monitor.UpdateAsync(monitor.Id,
                    new DogMonitorUpdateParameter("max(last_1m):avg:test.random{*} > 80")
                    {
                        Name = "metric alert edited",
                        Message = "test alert message edited",
                        Tags = tags2,
                        Options = new DogMonitorOptions()
                        {
                            IncludeTags = false,
                            TimeoutHours = 3,
                            NotifyAudit = false,
                            NotifyNoData = false,
                            Locked = false,
                            NewHostDelay = 150,
                            RenotifyInterval = 10,
                            NoDataTimeframe = 2,
                            EscalationMessage = "hogehoge edited",
                            RequireFullWindow = false,
                            EvaluationDelay = 3,
                            Thresholds = new Dictionary<string, double>()
                            {
                                {"critical", 80},
                                {"warning", 40},
                                {"critical_recovery", 60},
                                {"warning_recovery", 20},
                            },
                            Silenced = new Dictionary<string, DateTimeOffset?>()
                            {
                                {"*", silenced2}
                            }
                        }
                    });
                Debug.Assert(editMonitor.Query == "max(last_1m):avg:test.random{*} > 80");
                Debug.Assert(editMonitor.Name == "metric alert edited");
                Debug.Assert(editMonitor.Message == "test alert message edited");
                Debug.Assert(editMonitor.Tags.Length == tags2.Length);
                Debug.Assert(editMonitor.Tags[0] == tags2[0]);
                Debug.Assert(!editMonitor.Options.IncludeTags);
                Debug.Assert(editMonitor.Options.TimeoutHours == 3);
                Debug.Assert(!editMonitor.Options.NotifyAudit);
                Debug.Assert(editMonitor.Options.NotifyNoData == false);
                Debug.Assert(editMonitor.Options.Locked == false);
                Debug.Assert(editMonitor.Options.NewHostDelay == 150);
                Debug.Assert(editMonitor.Options.RenotifyInterval == 10);
                Debug.Assert(editMonitor.Options.NoDataTimeframe == 2);
                Debug.Assert(editMonitor.Options.EscalationMessage == "hogehoge edited");
                Debug.Assert(editMonitor.Options.RequireFullWindow == false);
                Debug.Assert(editMonitor.Options.Thresholds["critical"] == 80);
                Debug.Assert(editMonitor.Options.Thresholds["warning"] == 40);
                Debug.Assert(editMonitor.Options.Thresholds["critical_recovery"] == 60);
                Debug.Assert(editMonitor.Options.Thresholds["warning_recovery"] == 20);
                Debug.Assert(editMonitor.Options.Silenced["*"].Value.Normalize() ==  silenced2.Normalize());

                var getAllMonitors = await client.Monitor.GetAllAsync(groupStates: "all");
                getMonitor = getAllMonitors.FirstOrDefault(x => x.Id == editMonitor.Id);
                Debug.Assert(getMonitor.Query == "max(last_1m):avg:test.random{*} > 80");
                Debug.Assert(getMonitor.Name == "metric alert edited");
                Debug.Assert(getMonitor.Message == "test alert message edited");
                Debug.Assert(getMonitor.Tags.Length == tags2.Length);
                Debug.Assert(getMonitor.Tags[0] == tags2[0]);
                Debug.Assert(!editMonitor.Options.IncludeTags);
                Debug.Assert(editMonitor.Options.TimeoutHours == 3);
                Debug.Assert(!editMonitor.Options.NotifyAudit);
                Debug.Assert(getMonitor.Options.NotifyNoData == false);
                Debug.Assert(getMonitor.Options.Locked == false);
                Debug.Assert(getMonitor.Options.NewHostDelay == 150);
                Debug.Assert(getMonitor.Options.RenotifyInterval == 10);
                Debug.Assert(getMonitor.Options.NoDataTimeframe == 2);
                Debug.Assert(getMonitor.Options.EscalationMessage == "hogehoge edited");
                Debug.Assert(getMonitor.Options.RequireFullWindow == false);
                Debug.Assert(getMonitor.Options.Thresholds["critical"] == 80);
                Debug.Assert(getMonitor.Options.Thresholds["warning"] == 40);
                Debug.Assert(getMonitor.Options.Thresholds["critical_recovery"] == 60);
                Debug.Assert(getMonitor.Options.Thresholds["warning_recovery"] == 20);
                Debug.Assert(getMonitor.Options.Silenced["*"].Value.Normalize() == silenced2.Normalize());

                var unmute = await client.Monitor.UnmuteAsync(getMonitor.Id, "*", false);
                Debug.Assert(unmute.Options.Silenced.Count == 0);

                var muteend = DateTimeOffset.Now.AddHours(1);
                var mute = await client.Monitor.MuteAsync(getMonitor.Id, "*", muteend);
                Debug.Assert(mute.Options.Silenced["*"].Value.Normalize() == muteend.Normalize());

                var resolveResults = await client.Monitor.ResolveAsync(new [] { new DogMonitorResolve(getMonitor.Id, "ALL_GROUPS"), });

                var muteAll = await client.Monitor.MuteAllAsync();

                await client.Monitor.UnmuteAllAsync();

                foreach (var m in getAllMonitors)
                {
                    var deleteMonitor = await client.Monitor.DeleteAsync(m.Id);
                }

                getAllMonitors = await client.Monitor.GetAllAsync(groupStates: "all");
                Debug.Assert(getAllMonitors.Length == 0);
            }
        }

        private static DateTimeOffset Normalize(this DateTimeOffset @this)
        {
            @this = @this.UtcDateTime;
            return new DateTimeOffset(@this.Year, @this.Month, @this.Day, @this.Hour, @this.Minute, @this.Second, TimeSpan.Zero);
        }
    }
}