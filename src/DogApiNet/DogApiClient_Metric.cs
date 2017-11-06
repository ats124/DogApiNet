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

    public interface IMetricApi
    {
        Task<DogMetricGetListResult> GetListAsync(DateTimeOffset from, CancellationToken? cancelToken = null);

        Task<DogMetricPostResult> PostAsync(DogMetricPostParameter param, CancellationToken? cancelToken = null);
    }

    public class DogMetricGetListResult
    {
        [DataMember(Name = "metrics")]
        public string[] Metrics { get; set; }

        [DataMember(Name = "from")]
        [JsonFormatter(typeof(DurableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset From { get; set; }
    }

    public class DogMetricPostParameter
    {
        [DataMember(Name = "metric")]
        public string Metric { get; set; }

        [DataMember(Name = "points")]
        public DogMetricPoint[] Points { get; set; }

        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        public DogMetricPostParameter(string metric, DogMetricPoint[] points)
        {
            Metric = metric;
            Points = points;
        }

        public DogMetricPostParameter(string metric, DateTimeOffset timestamp, double value) : this(metric, new DogMetricPoint[] { new DogMetricPoint(timestamp, value) })
        {
        }
    }

    public class DogMetricPostResult
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
    }

    [JsonFormatter(typeof(DogMetricPointFormatter))]
    public class DogMetricPoint
    {
        public DateTimeOffset Timestamp { get; set; }
        public double Value { get; set; }

        public DogMetricPoint()
        {
        }

        public DogMetricPoint(DateTimeOffset timestamp, double value)
        {
            Timestamp = timestamp;
            Value = value;
        }
    }

    partial class DogApiClient : IMetricApi
    {
        public IMetricApi Metric => this;

        async Task<DogMetricGetListResult> IMetricApi.GetListAsync(DateTimeOffset from, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection()
            {
                { "from", from.ToUnixTimeSeconds().ToString() }
            };
            return await RequestAsync<DogMetricGetListResult>(HttpMethod.Get, $"/api/v1/metrics", @params, null, cancelToken);
        }

        async Task<DogMetricPostResult> IMetricApi.PostAsync(DogMetricPostParameter param, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(param, Utf8Json.Resolvers.StandardResolver.ExcludeNull));
            return await RequestAsync<DogMetricPostResult>(HttpMethod.Post, "/api/v1/series", null, data, cancelToken);
        }
    }
}
