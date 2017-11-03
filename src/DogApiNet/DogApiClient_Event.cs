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
        Task<DogEventQueryResult> QueryAsync(DateTimeOffset start, DateTimeOffset end, string priority = null, string[] tags = null, CancellationToken? cancelToken = null);
    }

    public class DogEventQueryResult
    {
        [DataMember(Name = "events")]
        public DogEvent[] Events { get; set; }
    }

    public class DogEvent
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

        public DogEvent()
        {
            _tagsAsDictionary = new Lazy<ILookup<string, string>>(() => 
                (Tags ?? Array.Empty<string>()).Select(x => DeconstructTag(x)).ToLookup(x => x.key, x => x.value));
        }

        [ThreadStatic]
        private static StringBuilder _buffer;
        public static (string key, string value) DeconstructTag(string tag)
        {
            if (_buffer == null || _buffer.Capacity > 256)
            {
                _buffer = new StringBuilder(256);
            }
            else
            {
                _buffer.Clear();
            }

            int i = 0;
            for (; i < tag.Length; i++)
            {
                if (tag[i] == ':')
                {
                    i++;
                    break;
                }
                else
                {
                    _buffer.Append(tag[i]);
                }
            }
            var key = _buffer.ToString();

            _buffer.Clear();
            for (; i < tag.Length; i++)
            {
                _buffer.Append(tag[i]);
            }
            var value = _buffer.ToString();

            return (key, value);
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
            return await RequestAsync<DogEventQueryResult>(HttpMethod.Get, "/api/v1/events", @params, null, cancelToken);
        }
    }

}
