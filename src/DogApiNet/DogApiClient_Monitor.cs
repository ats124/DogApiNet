using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DogApiNet.JsonFormatters;
using Utf8Json;

namespace DogApiNet
{
    public interface IMonitorApi
    {
        Task<DogMonitor> CreateAsync(DogMonitor monitor, CancellationToken? cancelToken = null);

        Task<DogMonitor> UpdateAsync(DogMonitor monitor, CancellationToken? cancelToken = null);

        Task<DogMonitor> GetAsync(long id, string groupStates = null, CancellationToken? cancelToken = null);

        Task<DogMonitor[]> GetAllAsync(string groupStates = null, string name = null, string tags = null,
            string monitorTags = null, bool? withDowntimes = null, CancellationToken? cancelToken = null);

        Task<DogMonitorDeleteResult> DeleteAsync(long id, CancellationToken? cancelToken = null);

        Task<DogMonitor[]> ResolveAsync(DogMonitorResolve[] resolves, CancellationToken? cancelToken = null);

        Task<DogMonitor> UnmuteAsync(long id, string scope = null, bool? allScopes = null,
            CancellationToken? cancelToken = null);

        Task<DogMonitor> MuteAsync(long id, string scope = null, DateTimeOffset? end = null,
            CancellationToken? cancelToken = null);

        Task<DogMonitorMuteAllResult> MuteAllAsync(CancellationToken? cancelToken = null);

        Task UnmuteAllAsync(CancellationToken? cancelToken = null);
    }

    public static class DogMonitorTypes
    {
        public static readonly string MericAlert = "metric alert";
        public static readonly string ServiceCheck = "service check";
        public static readonly string EventAlert = "event alert";
        public static readonly string Composite = "composite";
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogMonitor>))]
    public class DogMonitor : OptionalPropertySupport<DogMonitor>
    {
        internal DogMonitor()
        {
        }

        public DogMonitor(string type, string query)
        {
            Type = type;
            Query = query;
        }

        public DogMonitor(long id, string query)
        {
            Id = id;
            Query = query;
        }

        [DataMember(Name = "id")]
        public long Id
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        [DataMember(Name = "message")]
        public string Message
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "tags")]
        public string[] Tags
        {
            get => GetValue<string[]>();
            set => SetValue(value);
        }

        [DataMember(Name = "options")]
        public DogMonitorOptions Options
        {
            get => GetValue<DogMonitorOptions>();
            set => SetValue(value);
        }

        [DataMember(Name = "org_id")]
        public long OrgId
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        [DataMember(Name = "overall_state")]
        public string OverallState
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "overall_state_modified")]
        public DateTimeOffset? OverallStateModified
        {
            get => GetValue<DateTimeOffset?>();
            set => SetValue(value);
        }

        [DataMember(Name = "query")]
        public string Query
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "type")]
        public string Type
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "multi")]
        public bool Multi
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [DataMember(Name = "created")]
        public DateTimeOffset Created
        {
            get => GetValue<DateTimeOffset>();
            set => SetValue(value);
        }

        [DataMember(Name = "modified")]
        public DateTimeOffset Modified
        {
            get => GetValue<DateTimeOffset>();
            set => SetValue(value);
        }

        [DataMember(Name = "state")]
        public DogMonitorState State
        {
            get => GetValue<DogMonitorState>();
            set => SetValue(value);
        }

        public bool ShouldSerializeId() => false;

        public bool ShouldSerializeOrgId() => false;

        public bool ShouldSerializeOverallState() => false;

        public bool ShouldSerializeOverallStateModified() => false;

        public bool ShouldSerializeMulti() => false;

        public bool ShouldSerializeCreated() => false;

        public bool ShouldSerializeModified() => false;

        public bool ShouldSerializeState() => false;
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogMonitorOptions>))]
    public class DogMonitorOptions : OptionalPropertySupport<DogMonitorOptions>
    {
        [DataMember(Name = "silenced")]
        [JsonFormatter(typeof(StaticDictionaryFormatter<string, DateTimeOffset?, Dictionary<string, DateTimeOffset?>>),
            null, null, typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter), null)]
        public Dictionary<string, DateTimeOffset?> Silenced
        {
            get => GetValue<Dictionary<string, DateTimeOffset?>>();
            set => SetValue(value);
        }

        [DataMember(Name = "notify_no_data")]
        public bool NotifyNoData
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [DataMember(Name = "new_host_delay")]
        public int NewHostDelay
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        [DataMember(Name = "no_data_timeframe")]
        public int? NoDataTimeframe
        {
            get => GetValue<int?>();
            set => SetValue(value);
        }

        [DataMember(Name = "timeout_h")]
        public int? TimeoutHours
        {
            get => GetValue<int?>();
            set => SetValue(value);
        }

        [DataMember(Name = "require_full_window")]
        public bool RequireFullWindow
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [DataMember(Name = "renotify_interval")]
        public int? RenotifyInterval
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        [DataMember(Name = "escalation_message")]
        public string EscalationMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "notify_audit")]
        public bool NotifyAudit
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [DataMember(Name = "locked")]
        public bool Locked
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [DataMember(Name = "include_tags")]
        public bool IncludeTags
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [DataMember(Name = "thresholds")]
        public Dictionary<string, double> Thresholds
        {
            get => GetValue<Dictionary<string, double>>();
            set => SetValue(value);
        }

        [DataMember(Name = "evaluation_delay")]
        public int EvaluationDelay
        {
            get => GetValue<int>();
            set => SetValue(value);
        }
    }

    public class DogMonitorState
    {
        [DataMember(Name = "groups")]
        public Dictionary<string, DogMonitorStateGroup> Groups { get; set; }
    }

    public class DogMonitorStateGroup
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "triggering_value")]
        public DogMonitorTriggeringValue TriggeringValue { get; set; }

        [DataMember(Name = "last_triggered_ts")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? LastTriggered { get; set; }

        [DataMember(Name = "last_nodata_ts")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? LastNodata { get; set; }

        [DataMember(Name = "last_notified_ts")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? LastNotified { get; set; }

        [DataMember(Name = "last_resolved_ts")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? LastResolved { get; set; }
    }

    public class DogMonitorTriggeringValue
    {
        [DataMember(Name = "to_ts")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? To { get; set; }

        [DataMember(Name = "from_ts")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? From { get; set; }

        [DataMember(Name = "value")]
        public double Value { get; set; }
    }

    public class DogMonitorDeleteResult
    {
        [DataMember(Name = "deleted_monitor_id")]
        public long DeletedMonitorId { get; set; }
    }

    [JsonFormatter(typeof(DogMonitorResolveFormatter))]
    public class DogMonitorResolve
    {
        public DogMonitorResolve(long monitorId, string group)
        {
            MonitorId = monitorId;
            Group = group;
        }

        public long MonitorId { get; set; }
        public string Group { get; set; }
    }

    public class DogMonitorMuteAllResult
    {
        [DataMember(Name = "active")]
        public bool Active { get; set; }

        [DataMember(Name = "disable")]
        public bool Disabled { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "start")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? Start { get; set; }

        [DataMember(Name = "end")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? End { get; set; }

        [DataMember(Name = "scope")]
        public string[] Scope { get; set; }
    }

    partial class DogApiClient : IMonitorApi
    {
        public IMonitorApi Monitor => this;

        async Task<DogMonitor> IMonitorApi.CreateAsync(DogMonitor monitor, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(monitor));
            return await RequestAsync<DogMonitor>(HttpMethod.Post, "/api/v1/monitor", null, data, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogMonitor> IMonitorApi.UpdateAsync(DogMonitor monitor, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(monitor));
            return await RequestAsync<DogMonitor>(HttpMethod.Put, $"/api/v1/monitor/{monitor.Id}", null, data,
                cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorDeleteResult> IMonitorApi.DeleteAsync(long id, CancellationToken? cancelToken) =>
            await RequestAsync<DogMonitorDeleteResult>(HttpMethod.Delete, $"/api/v1/monitor/{id}", null, null,
                cancelToken).ConfigureAwait(false);

        async Task<DogMonitor> IMonitorApi.GetAsync(long id, string groupStates, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (groupStates != null) @params["group_states"] = groupStates;
            return await RequestAsync<DogMonitor>(HttpMethod.Get, $"/api/v1/monitor/{id}", @params, null, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogMonitor[]> IMonitorApi.GetAllAsync(string groupStates, string name, string tags,
            string monitorTags, bool? withDowntimes, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (groupStates != null) @params["group_states"] = groupStates;
            if (name != null) @params["name"] = name;
            if (tags != null) @params["tags"] = tags;
            if (monitorTags != null) @params["monitor_tags"] = monitorTags;
            if (withDowntimes != null) @params["with_downtimes"] = withDowntimes.Value.ToString().ToLowerInvariant();
            return await RequestAsync<DogMonitor[]>(HttpMethod.Get, "/api/v1/monitor", @params, null, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogMonitor[]> IMonitorApi.ResolveAsync(DogMonitorResolve[] resolves, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new {resolve = resolves}));
            return await RequestAsync<DogMonitor[]>(HttpMethod.Post, $"/monitor/bulk_resolve", null, data, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogMonitor> IMonitorApi.UnmuteAsync(long id, string scope, bool? allScopes,
            CancellationToken? cancelToken)
        {
            var dataDic = new Dictionary<string, object>();
            if (scope != null) dataDic["scope"] = scope;
            if (allScopes.HasValue) dataDic["all-scopes"] = allScopes;

            var data = dataDic.Count > 0
                ? new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(dataDic))
                : null;
            return await RequestAsync<DogMonitor>(HttpMethod.Post, $"/api/v1/monitor/{id}/unmute", null, data,
                cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitor> IMonitorApi.MuteAsync(long id, string scope, DateTimeOffset? end,
            CancellationToken? cancelToken)
        {
            var dataDic = new Dictionary<string, object>();
            if (scope != null) dataDic["scope"] = scope;
            if (end.HasValue) dataDic["end"] = end.Value.ToUnixTimeSeconds();

            var data = dataDic.Count > 0
                ? new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(dataDic))
                : null;
            return await RequestAsync<DogMonitor>(HttpMethod.Post, $"/api/v1/monitor/{id}/mute", null, data,
                cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorMuteAllResult> IMonitorApi.MuteAllAsync(CancellationToken? cancelToken) =>
            await RequestAsync<DogMonitorMuteAllResult>(HttpMethod.Post, $"/api/v1/monitor/mute_all", null,
                DogApiHttpRequestContent.EmptyJson,
                cancelToken).ConfigureAwait(false);

        async Task IMonitorApi.UnmuteAllAsync(CancellationToken? cancelToken)
        {
            await RequestAsync<NoJsonResponse>(HttpMethod.Post, $"/api/v1/monitor/unmute_all", null,
                    DogApiHttpRequestContent.EmptyJson, cancelToken)
                .ConfigureAwait(false);
        }
    }
}