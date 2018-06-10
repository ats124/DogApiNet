using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DogApiNet.Internal;
using DogApiNet.JsonFormatters;
using Utf8Json;
using Utf8Json.Resolvers;

namespace DogApiNet
{
    public interface IEventApi
    {
        Task<DogEventPostResult> PostAsync(DogEvent evt, CancellationToken? cancelToken = null);

        Task<DogEvent> GetAsync(long eventId, CancellationToken? cancelToken = null);

        Task<long> DeleteAsync(long eventId, CancellationToken? cancelToken = null);

        Task<DogEvent[]> QueryAsync(DateTimeOffset start, DateTimeOffset end, string priority = null,
            string[] tags = null, CancellationToken? cancelToken = null);
    }

    public class DogEvent
    {
        [IgnoreDataMember]
        private string[] _tags;

        [IgnoreDataMember]
        private Lazy<ILookup<string, string>> _tagsAsDictionary;

        public DogEvent()
        {
            InitializeTagsAsDictionary();
        }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "date_happened")]
        [JsonFormatter(typeof(UnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset DateHappened { get; set; } = DateTimeOffset.MinValue;

        [DataMember(Name = "priority")]
        public string Priority { get; set; }

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                InitializeTagsAsDictionary();
            }
        }

        [DataMember(Name = "alert_type")]
        public string AlertType { get; set; }

        [DataMember(Name = "aggregation_key")]
        public string AggregationKey { get; set; }

        [DataMember(Name = "source_type_name")]
        public string SourceTypeName { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "device_name")]
        public string DeviceName { get; set; }

        [DataMember(Name = "is_aggregate")]
        public bool IsAggregate { get; set; }

        [DataMember(Name = "children")]
        public DogChildEvent[] Children { get; set; }

        [DataMember(Name = "comments")]
        public string[] Comments { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [IgnoreDataMember]
        public ILookup<string, string> LookupTags => _tagsAsDictionary.Value;

        public bool ShouldSerializeId() => false;
        public bool ShouldSerializeDateHappened() => DateHappened != DateTimeOffset.MinValue;
        public bool ShouldSerializeResource() => false;
        public bool ShouldSerializeUrl() => false;
        public bool ShouldSerializeDeviceName() => false;
        public bool ShouldSerializeIsAggregate() => false;
        public bool ShouldSerializeComments() => false;
        public bool ShouldSerializeSource() => false;
        public bool ShouldSerializeChildren() => false;

        private void InitializeTagsAsDictionary()
        {
            if (_tagsAsDictionary == null || _tagsAsDictionary.IsValueCreated)
                _tagsAsDictionary = new Lazy<ILookup<string, string>>(() =>
                    (Tags ?? new string[0]).Select(DogApiClient.DeconstructTag)
                    .ToLookup(x => x.key, x => x.value));
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

    public class DogEventPostResult
    {
        [DataMember(Name = "event")]
        public DogEvent Event { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }
    }

    namespace Internal
    {
        public class DogEventGetResult
        {
            [DataMember(Name = "event")]
            public DogEvent Event { get; set; }
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
            public DogEvent[] Events { get; set; }
        }
    }

    partial class DogApiClient : IEventApi
    {
        public IEventApi Event => this;

        async Task<DogEventPostResult> IEventApi.PostAsync(DogEvent evt, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(evt, StandardResolver.ExcludeNull));
            var result =
                await RequestAsync<DogEventPostResult>(HttpMethod.Post, "/api/v1/events", null, data, cancelToken)
                    .ConfigureAwait(false);
            if (result.Event != null)
            {
                result.Event.Title = result.Event.Title ?? evt.Title;
                result.Event.Host = result.Event.Host ?? evt.Host;
                result.Event.AlertType = result.Event.AlertType ?? evt.AlertType;
            }

            return result;
        }

        async Task<DogEvent> IEventApi.GetAsync(long eventId, CancellationToken? cancelToken) =>
            (await RequestAsync<DogEventGetResult>(HttpMethod.Get, $"/api/v1/events/{eventId}", null, null, cancelToken)
                .ConfigureAwait(false)).Event;

        async Task<long> IEventApi.DeleteAsync(long eventId, CancellationToken? cancelToken) =>
            (await RequestAsync<DogEventDeleteResult>(HttpMethod.Delete, $"/api/v1/events/{eventId}", null, null,
                cancelToken).ConfigureAwait(false)).DeletedEventId;

        async Task<DogEvent[]> IEventApi.QueryAsync(DateTimeOffset start, DateTimeOffset end, string priority,
            string[] tags, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection
            {
                {"start", start.ToUnixTimeSeconds().ToString()},
                {"end", end.ToUnixTimeSeconds().ToString()}
            };
            if (priority != null) @params.Add("priority", priority);
            if (tags != null && tags.Length > 0) @params.Add("tags", string.Join(",", tags));
            return (await RequestAsync<DogEventQueryResult>(HttpMethod.Get, "/api/v1/events", @params, null,
                cancelToken).ConfigureAwait(false)).Events;
        }
    }
}