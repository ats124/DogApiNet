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

    public interface IDowntimeApi
    {
        Task<DogDowntimeCreateResult> CreateAsync(DogDowntimeCreateParameter param, CancellationToken? cancelToken = null);
        Task<DogDowntimeCreateResult> UpdateAsync(DogDowntimeUpdateParameter param, CancellationToken? cancelToken = null);
        Task<DogDowntimeCancelByScopeResult> CancelByScopeAsync(string[] scope, CancellationToken? cancelToken = null);
        Task DeleteAsync(long id, CancellationToken? cancelToken = null);
        Task<DogDowntimeGetResult> GetAsync(long id, CancellationToken? cancelToken = null);
        Task<DogDowntimeGetResult[]> GetAllAsync(bool? currentOnly = null, CancellationToken? cancelToken = null);
    }

    public static class DogDowntimeRecurrenceTypes
    {
        public static readonly string Days = "days";
        public static readonly string Weeks = "weeks";
        public static readonly string Months = "months";
        public static readonly string Years = "years";
    }

    public abstract class DogDowntimeCreateUpateParameterBase<T> : OptionalPropertySupport<T> where T : OptionalPropertySupport<T>
    {
        [DataMember(Name = "scope")]
        public string[] Scope { get => GetValue<string[]>(); set => SetValue(value); }

        [DataMember(Name = "monitor_id")]
        public long? MonitorId { get => GetValue<long?>(); set => SetValue(value); }

        [DataMember(Name = "start")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? Start { get => GetValue<DateTimeOffset?>(); set => SetValue(value); }

        [DataMember(Name = "end")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? End  { get => GetValue<DateTimeOffset?>(); set => SetValue(value); }

        [DataMember(Name = "message")]
        public string Message { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "recurrence")]
        public DogDowntimeRecurrence Recurrence { get => GetValue<DogDowntimeRecurrence>(); set => SetValue(value); }

        [DataMember(Name = "timezone")]
        public string TimeZone { get => GetValue<string>(); set => SetValue(value); }
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogDowntimeCreateParameter>))]
    public class DogDowntimeCreateParameter : DogDowntimeCreateUpateParameterBase<DogDowntimeCreateParameter>
    {
        public DogDowntimeCreateParameter()
        {                
        }

        public DogDowntimeCreateParameter(params string[] scope)
        {
            Scope = scope;
        }
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogDowntimeUpdateParameter>))]
    public class DogDowntimeUpdateParameter : DogDowntimeCreateUpateParameterBase<DogDowntimeUpdateParameter>
    {
        [DataMember(Name = "id")]
        public long Id { get => GetValue<long>(); set => SetValue(value); }

        public DogDowntimeUpdateParameter()
        {                
        }

        public DogDowntimeUpdateParameter(long id, params string[] scope)
        {
            Id = id;
            Scope = scope;
        }
    }

    public class DogDowntimeCreateResult : DogDowntimeGetResult
    {
    }

    public class DogDowntimeCancelByScopeResult
    {
        [DataMember(Name = "cancelled_ids")]
        public long[] CancelledIds { get; set; }
    }

    [JsonFormatter(typeof(OptionalPropertySupportFormatter<DogDowntimeRecurrence>))]
    public class DogDowntimeRecurrence : OptionalPropertySupport<DogDowntimeRecurrence>
    {
        [DataMember(Name = "type")]
        public string Type { get => GetValue<string>(); set => SetValue(value); }

        [DataMember(Name = "period")]
        public int Period { get => GetValue<int>(); set => SetValue(value); }

        [DataMember(Name = "week_days")]
        [JsonFormatter(typeof(WeekDaysFormatter))]
        public DayOfWeek[] WeekDays { get => GetValue<DayOfWeek[]>(); set => SetValue(value); }

        [DataMember(Name = "until_date")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? UntilDate { get => GetValue<DateTimeOffset?>(); set => SetValue(value); }

        [DataMember(Name = "until_occurrences")]
        public int? UntilOccurrences { get => GetValue<int?>(); set => SetValue(value); }

        public DogDowntimeRecurrence()
        {            
        }

        public DogDowntimeRecurrence(string type, int period)
        {
            Type = type;
            Period = period;
        }
    }

    public class DogDowntimeGetResult
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "active")]
        public bool Active { get; set; }

        [DataMember(Name = "canceled")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? Canceled { get; set; }

        [DataMember(Name = "creator_id")]
        public long? CreatorId { get; set; }

        [DataMember(Name = "updater_id")]
        public long? UpdaterId { get; set; }

        [DataMember(Name = "parent_id")]
        public long? ParentId { get; set; }

        [DataMember(Name = "disabled")]
        public bool Disabled { get; set; }

        [DataMember(Name = "scope")]
        public string[] Scope { get; set; }

        [DataMember(Name = "monitor_id")]
        public long? MonitorId { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "start")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? Start { get; set; }

        [DataMember(Name = "end")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? End { get; set; }

        [DataMember(Name = "recurrence")]
        public DogDowntimeRecurrence Recurrence { get; set; }

        [DataMember(Name = "timezone")]
        public string TimeZone { get; set; }
    }

    partial class DogApiClient : IDowntimeApi
    {
        public IDowntimeApi Downtime => this;

        async Task<DogDowntimeCreateResult> IDowntimeApi.CreateAsync(DogDowntimeCreateParameter param, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(param));
            return await RequestAsync<DogDowntimeCreateResult>(HttpMethod.Post, $"/api/v1/downtime", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogDowntimeCreateResult> IDowntimeApi.UpdateAsync(DogDowntimeUpdateParameter param, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(param));
            return await RequestAsync<DogDowntimeCreateResult>(HttpMethod.Post, $"/api/v1/downtime", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task IDowntimeApi.DeleteAsync(long id, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", new byte[0]);
            await RequestAsync<NoJsonResponse>(HttpMethod.Delete, $"/api/v1/downtime/{id}", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogDowntimeCancelByScopeResult> IDowntimeApi.CancelByScopeAsync(string[] scope, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(new Dictionary<string, object>{ {"scope", scope} }));
            return await RequestAsync<DogDowntimeCancelByScopeResult>(HttpMethod.Post, $"/api/v1/downtime/cancel/by_scope", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogDowntimeGetResult> IDowntimeApi.GetAsync(long id, CancellationToken? cancelToken)
        {
            return await RequestAsync<DogDowntimeGetResult>(HttpMethod.Get, $"/api/v1/downtime/{id}", null, null, cancelToken).ConfigureAwait(false);
        }

        async Task<DogDowntimeGetResult[]> IDowntimeApi.GetAllAsync(bool? currentOnly, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (currentOnly != null)
            {
                @params.Add("current_only", currentOnly.Value.ToString().ToLowerInvariant());
            }
            return await RequestAsync<DogDowntimeGetResult[]>(HttpMethod.Get, "/api/v1/downtime", @params, null, cancelToken).ConfigureAwait(false);
        }
    }
}
