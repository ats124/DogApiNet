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
    public interface IDowntimeApi
    {
        Task<DogDowntime> CreateAsync(DogDowntime downtime, CancellationToken? cancelToken = null);
        Task<DogDowntime> UpdateAsync(DogDowntime downtime, CancellationToken? cancelToken = null);
        Task<DogDowntimeCancelByScopeResult> CancelByScopeAsync(string[] scope, CancellationToken? cancelToken = null);
        Task DeleteAsync(long id, CancellationToken? cancelToken = null);
        Task<DogDowntime> GetAsync(long id, CancellationToken? cancelToken = null);
        Task<DogDowntime[]> GetAllAsync(bool? currentOnly = null, CancellationToken? cancelToken = null);
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogDowntime>))]
    public class DogDowntime : OptionalPropertySupport<DogDowntime>
    {
        internal DogDowntime()
        {
        }

        public DogDowntime(params string[] scope)
        {
            Scope = scope;
        }

        public DogDowntime(long id, params string[] scope)
        {
            Id = id;
            Scope = scope;
        }

        [DataMember(Name = "id")]
        public long Id
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        [DataMember(Name = "scope")]
        public string[] Scope
        {
            get => GetValue<string[]>();
            set => SetValue(value);
        }

        [DataMember(Name = "monitor_id")]
        public long? MonitorId
        {
            get => GetValue<long?>();
            set => SetValue(value);
        }

        [DataMember(Name = "start")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? Start
        {
            get => GetValue<DateTimeOffset?>();
            set => SetValue(value);
        }

        [DataMember(Name = "end")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? End
        {
            get => GetValue<DateTimeOffset?>();
            set => SetValue(value);
        }

        [DataMember(Name = "message")]
        public string Message
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "recurrence")]
        public DogDowntimeRecurrence Recurrence
        {
            get => GetValue<DogDowntimeRecurrence>();
            set => SetValue(value);
        }

        [DataMember(Name = "timezone")]
        public string TimeZone
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "active")]
        public bool Active
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        [DataMember(Name = "canceled")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? Canceled
        {
            get => GetValue<DateTimeOffset?>();
            set => SetValue(value);
        }

        [DataMember(Name = "creator_id")]
        public long? CreatorId
        {
            get => GetValue<long?>();
            set => SetValue(value);
        }

        [DataMember(Name = "updater_id")]
        public long? UpdaterId
        {
            get => GetValue<long?>();
            set => SetValue(value);
        }

        [DataMember(Name = "parent_id")]
        public long? ParentId
        {
            get => GetValue<long?>();
            set => SetValue(value);
        }

        [DataMember(Name = "disabled")]
        public bool Disabled
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool ShouldSerializeId() => false;
        public bool ShouldSerializeActive() => false;
        public bool ShouldSerializeCanceled() => false;
        public bool ShouldSerializeCreatorId() => false;
        public bool ShouldSerializeUpdaterId() => false;
        public bool ShouldSerializeParentId() => false;
        public bool ShouldSerializeDisabled() => false;
    }

    public static class DogDowntimeRecurrenceTypes
    {
        public static readonly string Days = "days";
        public static readonly string Weeks = "weeks";
        public static readonly string Months = "months";
        public static readonly string Years = "years";
    }

    public class DogDowntimeCancelByScopeResult
    {
        [DataMember(Name = "cancelled_ids")]
        public long[] CancelledIds { get; set; }
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogDowntimeRecurrence>))]
    public class DogDowntimeRecurrence : OptionalPropertySupport<DogDowntimeRecurrence>
    {
        internal DogDowntimeRecurrence()
        {
        }

        public DogDowntimeRecurrence(string type, int period)
        {
            Type = type;
            Period = period;
        }

        [DataMember(Name = "type")]
        public string Type
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        [DataMember(Name = "period")]
        public int Period
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        [DataMember(Name = "week_days")]
        [JsonFormatter(typeof(WeekDaysFormatter))]
        public DayOfWeek[] WeekDays
        {
            get => GetValue<DayOfWeek[]>();
            set => SetValue(value);
        }

        [DataMember(Name = "until_date")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? UntilDate
        {
            get => GetValue<DateTimeOffset?>();
            set
            {
                SetValue(value);
                Unset(x => x.UntilOccurrences);
            }
        }

        [DataMember(Name = "until_occurrences")]
        public int? UntilOccurrences
        {
            get => GetValue<int?>();
            set
            {
                SetValue(value);
                Unset(x => x.UntilDate);
            }
        }
    }

    partial class DogApiClient : IDowntimeApi
    {
        public IDowntimeApi Downtime => this;

        async Task<DogDowntime> IDowntimeApi.CreateAsync(DogDowntime downtime, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(downtime));
            return await RequestAsync<DogDowntime>(HttpMethod.Post, $"/api/v1/downtime", null, data, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogDowntime> IDowntimeApi.UpdateAsync(DogDowntime downtime, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(downtime));
            return await RequestAsync<DogDowntime>(HttpMethod.Put, $"/api/v1/downtime/{downtime.Id}", null, data,
                cancelToken).ConfigureAwait(false);
        }

        async Task IDowntimeApi.DeleteAsync(long id, CancellationToken? cancelToken)
        {
            await RequestAsync<NoJsonResponse>(HttpMethod.Delete, $"/api/v1/downtime/{id}", null,
                DogApiHttpRequestContent.EmptyJson, cancelToken).ConfigureAwait(false);
        }

        async Task<DogDowntimeCancelByScopeResult> IDowntimeApi.CancelByScopeAsync(string[] scope,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new Dictionary<string, object> {{"scope", scope}}));
            return await RequestAsync<DogDowntimeCancelByScopeResult>(HttpMethod.Post,
                $"/api/v1/downtime/cancel/by_scope", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogDowntime> IDowntimeApi.GetAsync(long id, CancellationToken? cancelToken) =>
            await RequestAsync<DogDowntime>(HttpMethod.Get, $"/api/v1/downtime/{id}", null, null, cancelToken)
                .ConfigureAwait(false);

        async Task<DogDowntime[]> IDowntimeApi.GetAllAsync(bool? currentOnly, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (currentOnly != null) @params.Add("current_only", currentOnly.Value.ToString().ToLowerInvariant());
            return await RequestAsync<DogDowntime[]>(HttpMethod.Get, "/api/v1/downtime", @params, null, cancelToken)
                .ConfigureAwait(false);
        }
    }
}