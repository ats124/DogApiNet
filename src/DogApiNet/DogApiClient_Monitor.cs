using DogApiNet.JsonFormatters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace DogApiNet
{
    public interface IMonitorApi
    {
        Task<Monitor> GetMonitorAsync(int id, CancellationToken? cancelToken = null);
        Task<Monitor[]> GetMonitorByNameAsync(string name, CancellationToken? cancelToken = null);
        Task<Monitor[]> GetMonitorByTagsAsync(string[] tags, CancellationToken? cancelToken = null);
        Task<Monitor[]> GetMonitorListAsync(CancellationToken? cancelToken = null);
    }

    public class Monitor
    {
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "deleted")]
        public string Deleted { get; set; }

        [DataMember(Name = "query")]
        public string Query { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "matching_downtimes")]
        public object[] MatchingDowntimes { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "multi")]
        public bool Multi { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "created")]
        public string Created { get; set; }

        [DataMember(Name = "created_at")]
        [JsonFormatter(typeof(UnixTimeMillisecondsDateTimeOffsetFormatter))]
        public DateTimeOffset CreatedAt { get; set; }

        [DataMember(Name = "creator")]
        public Creator Creator { get; set; }

        [DataMember(Name = "org_id")]
        public int OrgId { get; set; }

        [DataMember(Name = "modified")]
        public string Modified { get; set; }

        [DataMember(Name = "overall_state_modified")]
        public string OverallStateModified { get; set; }

        [DataMember(Name = "overall_state")]
        public string OverallState { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "options")]
        public Options Options { get; set; }
    }

    public class Options
    {
        [DataMember(Name = "no_data_timeframe")]
        public int? NoDataTimeframe { get; set; }

        [DataMember(Name = "notify_audit")]
        public bool NotifyAudit { get; set; }

        [DataMember(Name = "notify_no_data")]
        public bool NotifyNoData { get; set; }

        [DataMember(Name = "renotify_interval")]
        public int RenotifyInterval { get; set; }

        [DataMember(Name = "new_host_delay")]
        public int NewHostDelay { get; set; }

        [DataMember(Name = "evaluation_delay")]
        public int EvaluationDelay { get; set; }

        [DataMember(Name = "silenced")]
        public Dictionary<string, string> Silenced { get; set; }

        [DataMember(Name = "timeout_h")]
        public int TimeoutH { get; set; }

        [DataMember(Name = "escalation_message")]
        public string EscalationMessage { get; set; }

        [DataMember(Name = "thresholds")]
        public Dictionary<string, double> Thresholds { get; set; }

        [DataMember(Name = "include_tags")]
        public bool IncludeTags { get; set; }

        [DataMember(Name = "require_full_window")]
        public bool RequireFullWindow { get; set; }

        [DataMember(Name = "locked")]
        public bool Locked { get; set; }
    }

    public class Creator
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "handle")]
        public string Handle { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    public class ThresholdCount
    {
        [DataMember(Name = "ok")]
        public int Ok { get; set; }
        [DataMember(Name = "critical")]
        public int Critical { get; set; }
        [DataMember(Name = "warning")]
        public int Warning { get; set; }
        [DataMember(Name = "unknown")]
        public int Unknown { get; set; }
        [DataMember(Name = "critical_recovery")]
        public int CriticalRecovery { get; set; }
        [DataMember(Name = "warning_recovery")]
        public int WarningRecovery { get; set; }
    }

    partial class DogApiClient : IMonitorApi
    {
        public IMonitorApi Monitor => this;

        async Task<Monitor> IMonitorApi.GetMonitorAsync(int id, CancellationToken? cancelToken = null)
        {
            return await RequestAsync<Monitor>(HttpMethod.Get, $"/api/v1/monitor/{id}", null, null, cancelToken).ConfigureAwait(false);
        }

        async Task<Monitor[]> IMonitorApi.GetMonitorByNameAsync(string name, CancellationToken? cancelToken = null)
        {
            var @param = new NameValueCollection {
                { "name", name}
            };

            return await RequestAsync<Monitor[]>(HttpMethod.Get, $"/api/v1/monitor", @param, null, cancelToken).ConfigureAwait(false);
        }

        async Task<Monitor[]> IMonitorApi.GetMonitorByTagsAsync(string[] tags, CancellationToken? cancelToken = null)
        {
            var @param = new NameValueCollection {
                { "monitor_tags", string.Join(",", tags)}
            };

            return await RequestAsync<Monitor[]>(HttpMethod.Get, $"/api/v1/monitor", @param, null, cancelToken).ConfigureAwait(false);
        }

        async Task<Monitor[]> IMonitorApi.GetMonitorListAsync(CancellationToken? cancelToken)
        {
            return await RequestAsync<Monitor[]>(HttpMethod.Get, $"/api/v1/monitor", null, null, cancelToken).ConfigureAwait(false);
        }
    }
}