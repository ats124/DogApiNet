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
    }

    public class DogDowntimeGetResult
    {
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
