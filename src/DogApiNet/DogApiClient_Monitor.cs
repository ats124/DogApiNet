using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Formatters;

namespace DogApiNet
{
    using JsonFormatters;

    public interface IMonitorApi
    {
        Task<DogMonitorCreateResult> CreateAsync(DogMonitorCreateParameter param, CancellationToken? cancelToken = null);

        Task<DogMonitorCreateResult> EditAsync(long id, DogMonitorEditParameter param, CancellationToken? cancelToken = null);

        Task<DogMonitorGetResult> GetAsync(long id, string groupStates = null, CancellationToken? cancelToken = null);

        Task<DogMonitorGetResult[]> GetAllAsync(string groupStates = null, string name = null, string tags = null, string monitorTags = null, bool? withDowntimes = null, CancellationToken? cancelToken = null);

        Task<DogMonitorDeleteResult> DeleteAsync(long id, CancellationToken? cancelToken = null);

        Task<DogMonitorResolveResult[]> ResolveAsync(DogMonitorResolve[] resolves, CancellationToken? cancelToken = null);

        Task<DogMonitorUnmuteResult> UnmuteAsync(long id, string scope = null, bool? allScopes = null, CancellationToken? cancelToken = null);

        Task<DogMonitorMuteResult> MuteAsync(long id, string scope = null, DateTimeOffset? end = null, CancellationToken? cancelToken = null);

        Task<DogMonitorMuteAllResult> MuteAllAsync(CancellationToken? cancelToken = null);
    }

    public static class DogMonitorTypes
    {
        public static readonly string MericAlert = "metric alert";
        public static readonly string ServiceCheck = "service check";
        public static readonly string EventAlert = "event alert";
        public static readonly string Composite = "composite";
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogMonitorCreateParameter>))]
    public class DogMonitorCreateParameter : OptionalPropertySupport<DogMonitorCreateParameter>
    {
        [DataMember(Name = "type")]
        public string Type { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "query")]
        public string Query { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "name")]
        public string Name { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "message")]
        public string Message { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "tags")]
        public string[] Tags { get => GetValue<string[]>(); set => SetValue(value); }

        [DataMember(Name = "options")]
        public DogMonitorOptions Options { get => GetValue<DogMonitorOptions>(); set => SetValue(value); }

        public DogMonitorCreateParameter()
        {
        }

        public DogMonitorCreateParameter(string type, string query)
        {
            Type = type;
            Query = query;
        }
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogMonitorEditParameter>))]
    public class DogMonitorEditParameter : OptionalPropertySupport<DogMonitorEditParameter>
    {
        [DataMember(Name = "query")]
        public string Query { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "name")]
        public string Name { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "message")]
        public string Message { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "tags")]
        public string[] Tags { get => GetValue<string[]>(); set => SetValue(value); }

        [DataMember(Name = "options")]
        public DogMonitorOptions Options { get => GetValue<DogMonitorOptions>(); set => SetValue(value); }

        public DogMonitorEditParameter()
        {
        }

        public DogMonitorEditParameter(string query)
        {
            Query = query;
        }
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogMonitorOptions>))]
    public class DogMonitorOptions : OptionalPropertySupport<DogMonitorOptions>
    {
        [DataMember(Name = "silenced")]
        [JsonFormatter(typeof(StaticDictionaryFormatter<string, DateTimeOffset?, Dictionary<string, DateTimeOffset?>>), null, null, typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter), null)]
        public Dictionary<string, DateTimeOffset?> Silenced { get => GetValue<Dictionary<string, DateTimeOffset?>>(); set => SetValue(value); }

        [DataMember(Name = "notify_no_data")]
        public bool NotifyNoData { get => GetValue<bool>(); set => SetValue(value); }

        [DataMember(Name = "new_host_delay")]
        public int NewHostDelay { get => GetValue<int>(); set => SetValue(value); }

        [DataMember(Name = "no_data_timeframe")]
        public int? NoDataTimeframe { get => GetValue<int?>(); set => SetValue(value); }

        [DataMember(Name = "timeout_h")]
        public int? TimeoutHours { get => GetValue<int?>(); set => SetValue(value); }

        [DataMember(Name = "require_full_window")]
        public bool RequireFullWindow { get => GetValue<bool>(); set => SetValue(value); }

        [DataMember(Name = "renotify_interval")]
        public int? RenotifyInterval { get => GetValue<int>(); set => SetValue(value); }

        [DataMember(Name = "escalation_message")]
        public string EscalationMessage { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "notify_audit")]
        public bool NotifyAudit { get => GetValue<bool>(); set => SetValue(value); }

        [DataMember(Name = "locked")]
        public bool Locked { get => GetValue<bool>(); set => SetValue(value); }

        [DataMember(Name = "include_tags")]
        public bool IncludeTags { get => GetValue<bool>(); set => SetValue(value); }

        [DataMember(Name = "thresholds")]
        public Dictionary<string, double> Thresholds { get => GetValue<Dictionary<string, double>>(); set => SetValue(value); }

        [DataMember(Name = "evaluation_delay")]
        public int EvaluationDelay { get => GetValue<int>(); set => SetValue(value); }
    }

    public class DogMonitorCreateResult
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "options")]
        public DogMonitorOptions Options { get; set; }

        [DataMember(Name = "org_id")]
        public long OrgId { get; set; }

        [DataMember(Name = "overall_state")]
        public string OverallState { get; set; }

        [DataMember(Name = "query")]
        public string Query { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "multi")]
        public bool Multi { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset Modified { get; set; }
    }

    public class DogMonitorGetResult
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "options")]
        public DogMonitorOptions Options { get; set; }

        [DataMember(Name = "org_id")]
        public long OrgId { get; set; }

        [DataMember(Name = "overall_state")]
        public string OverallState { get; set; }

        [DataMember(Name = "overall_state_modified")]
        public DateTimeOffset? OverallStateModified { get; set; }

        [DataMember(Name = "query")]
        public string Query { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "multi")]
        public bool Multi { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset Modified { get; set; }

        [DataMember(Name = "state")]
        public DogMonitorState State { get; set; }
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
        public long MonitorId { get; set; }
        public string Group { get; set; }

        public DogMonitorResolve(long monitorId, string group)
        {
            MonitorId = monitorId;
            Group = group;
        }
    }

    public class DogMonitorResolveResult : DogMonitorGetResult
    {
    }

    public class DogMonitorUnmuteResult : DogMonitorGetResult
    {
    }

    public class DogMonitorMuteResult : DogMonitorGetResult
    {
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

        async Task<DogMonitorCreateResult> IMonitorApi.CreateAsync(DogMonitorCreateParameter param, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(param));
            return await RequestAsync<DogMonitorCreateResult>(HttpMethod.Post, "/api/v1/monitor", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorCreateResult> IMonitorApi.EditAsync(long id, DogMonitorEditParameter param, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(param));
            return await RequestAsync<DogMonitorCreateResult>(HttpMethod.Put, $"/api/v1/monitor/{id}", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorDeleteResult> IMonitorApi.DeleteAsync(long id, CancellationToken? cancelToken)
        {
            return await RequestAsync<DogMonitorDeleteResult>(HttpMethod.Delete, $"/api/v1/monitor/{id}", null, null, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorGetResult> IMonitorApi.GetAsync(long id, string groupStates, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (groupStates != null) @params["group_states"] = groupStates;
            return await RequestAsync<DogMonitorGetResult>(HttpMethod.Get, $"/api/v1/monitor/{id}", @params, null, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorGetResult[]> IMonitorApi.GetAllAsync(string groupStates, string name, string tags, string monitorTags, bool? withDowntimes, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (groupStates != null) @params["group_states"] = groupStates;
            if (name != null) @params["name"] = name;
            if (tags != null) @params["tags"] = tags;
            if (monitorTags != null) @params["monitor_tags"] = monitorTags;
            if (withDowntimes != null) @params["with_downtimes"] = withDowntimes.Value.ToString().ToLowerInvariant();
            return await RequestAsync<DogMonitorGetResult[]>(HttpMethod.Get, "/api/v1/monitor", @params, null, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorResolveResult[]> IMonitorApi.ResolveAsync(DogMonitorResolve[] resolves, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(new { resolve = resolves }));
            return await RequestAsync<DogMonitorResolveResult[]>(HttpMethod.Post, $"/monitor/bulk_resolve", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorUnmuteResult> IMonitorApi.UnmuteAsync(long id, string scope, bool? allScopes, CancellationToken? cancelToken)
        {
            var dataDic = new Dictionary<string, object>();
            if (scope != null) dataDic["scope"] = scope;
            if (allScopes.HasValue) dataDic["all-scopes"] = allScopes;

            var data = dataDic.Count > 0
                ? new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(dataDic))
                : null;
            return await RequestAsync<DogMonitorUnmuteResult>(HttpMethod.Post, $"/api/v1/monitor/{id}/unmute", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorMuteResult> IMonitorApi.MuteAsync(long id, string scope, DateTimeOffset? end, CancellationToken? cancelToken)
        {
            var dataDic = new Dictionary<string, object>();
            if (scope != null) dataDic["scope"] = scope;
            if (end.HasValue) dataDic["end"] = end.Value.ToUnixTimeSeconds();

            var data = dataDic.Count > 0
                ? new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(dataDic))
                : null;
            return await RequestAsync<DogMonitorMuteResult>(HttpMethod.Post, $"/api/v1/monitor/{id}/mute", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMonitorMuteAllResult> IMonitorApi.MuteAllAsync(CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", new byte[0]);
            return await RequestAsync<DogMonitorMuteAllResult>(HttpMethod.Post, $"/api/v1/monitor/mute_all", null, data, cancelToken).ConfigureAwait(false);
        }
    }
}
