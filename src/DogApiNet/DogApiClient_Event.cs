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

    public interface IEventApi
    {
        Task<DogEventPostResult> PostAsync(DogEventPostParameter param, CancellationToken? cancelToken = null);

        Task<DogEventGetResult> GetAsync(long eventId, CancellationToken? cancelToken = null);

        Task<DogEventDeleteResult> DeleteAsync(long eventId, CancellationToken? cancelToken = null);

        Task<DogEventQueryResult> QueryAsync(DateTimeOffset start, DateTimeOffset end, string priority = null, string[] tags = null, CancellationToken? cancelToken = null);
    }

    public class DogEventPostParameter
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "date_happened")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? DateHappened { get; set; }

        [DataMember(Name = "priority")]
        public string Priority { get; set; }

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "alert_type")]
        public string AlertType { get; set; }

        [DataMember(Name = "aggregation_key")]
        public string AggregationKey { get; set; }

        [DataMember(Name = "source_type_name")]
        public string SourceTypeName { get; set; }

        public DogEventPostParameter(string title)
        {
            Title = title;
        }
    }

    public class DogEventPostResult
    {
        [DataMember(Name = "event")]
        public DogEventPostResultEvent Event { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }
    }

    public class DogEventPostResultEvent
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "date_happened")]
        [JsonFormatter(typeof(UnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset DateHappened { get; set; }

        [DataMember(Name = "priority")]
        public string Priority { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
    }

    public class DogEventGetResult
    {
        [DataMember(Name = "event")]
        public DogEventGetResultEvent Event { get; set; }
    }

    public class DogEventGetResultEvent
    {
        [DataMember(Name = "date_happened")]
        [JsonFormatter(typeof(UnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset DateHappened { get; set; }

        [DataMember(Name = "alert_type")]
        public string AlertType { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "device_name")]
        public string DeviceName { get; set; }

        [DataMember(Name = "priority")]
        public string Priority { get; set; }

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [IgnoreDataMember]
        private Lazy<ILookup<string, string>> _tagsAsDictionary;
        [IgnoreDataMember]
        public ILookup<string, string> LookupTags => _tagsAsDictionary.Value;

        public DogEventGetResultEvent()
        {
            _tagsAsDictionary = new Lazy<ILookup<string, string>>(() =>
                (Tags ?? new string[0]).Select(x => DogApiClient.DeconstructTag(x)).ToLookup(x => x.key, x => x.value));
        }
    }

    public class DogEventDeleteResult
    {
        [DataMember(Name = "deleted_event_id")]
        [JsonFormatter(typeof(DurableInt64Formatter))]
        public long DeletedEventId { get; set; }
    }

    public class DogEventQueryResult
    {
        [DataMember(Name = "events")]
        public DogEventQueryResultEvent[] Events { get; set; }
    }

    public class DogEventQueryResultEvent
    {
        [DataMember(Name = "date_happened")]
        [JsonFormatter(typeof(UnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset DateHappened { get; set; }

        [DataMember(Name = "alert_type")]
        public string AlertType { get; set; }

        [DataMember(Name = "is_aggregate")]
        public bool IsAggregate { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "comments")]
        public string[] Comments { get; set; }

        [DataMember(Name = "device_name")]
        public string DeviceName { get; set; }

        [DataMember(Name = "children")]
        public DogChildEvent[] Children { get; set; }

        [DataMember(Name = "priority")]
        public string Priority { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [IgnoreDataMember]
        private Lazy<ILookup<string, string>> _tagsAsDictionary;
        [IgnoreDataMember]
        public ILookup<string, string> LookupTags => _tagsAsDictionary.Value;

        public DogEventQueryResultEvent()
        {
            _tagsAsDictionary = new Lazy<ILookup<string, string>>(() => 
                (Tags ?? new string[0]).Select(x => DogApiClient.DeconstructTag(x)).ToLookup(x => x.key, x => x.value));
        }
    }

    public class DogChildEvent
    {
        [DataMember(Name = "date_happened")]
        [JsonFormatter(typeof(UnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset DateHappened { get; set; }
        [DataMember(Name = "alert_type")]
        public string AlertType { get; set; }
        [DataMember(Name = "id")]
        [JsonFormatter(typeof(DurableInt64Formatter))]
        public long Id { get; set; }
    }

    partial class DogApiClient : IEventApi
    {
        public IEventApi Event => this;

        async Task<DogEventPostResult> IEventApi.PostAsync(DogEventPostParameter param, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(param, Utf8Json.Resolvers.StandardResolver.ExcludeNull));
            return await RequestAsync<DogEventPostResult>(HttpMethod.Post, "/api/v1/events", null, data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogEventGetResult> IEventApi.GetAsync(long eventId, CancellationToken? cancelToken)
        {
            return await RequestAsync<DogEventGetResult>(HttpMethod.Get, $"/api/v1/events/{eventId}", null, null, cancelToken).ConfigureAwait(false);
        }

        async Task<DogEventDeleteResult> IEventApi.DeleteAsync(long eventId, CancellationToken? cancelToken)
        {
            return await RequestAsync<DogEventDeleteResult>(HttpMethod.Delete, $"/api/v1/events/{eventId}", null, null, cancelToken).ConfigureAwait(false);
        }

        async Task<DogEventQueryResult> IEventApi.QueryAsync(DateTimeOffset start, DateTimeOffset end, string priority, string[] tags, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection()
            {
                { "start", start.ToUnixTimeSeconds().ToString() },
                { "end", end.ToUnixTimeSeconds().ToString() },
            };
            if (priority != null)
            {
                @params.Add("priority", priority);
            }
            if (tags != null && tags.Length > 0)
            {
                @params.Add("tags", string.Join(",", tags));
            }
            return await RequestAsync<DogEventQueryResult>(HttpMethod.Get, "/api/v1/events", @params, null, cancelToken).ConfigureAwait(false);
        }
    }

}
