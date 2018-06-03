using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DogApiNet.JsonFormatters;
using Utf8Json;
using Utf8Json.Resolvers;

namespace DogApiNet
{
    public interface IMetricApi
    {
        Task<DogMetricGetListResult> GetListAsync(DateTimeOffset from, CancellationToken? cancelToken = null);

        Task<DogMetricPostResult> PostAsync(DogMetric[] metrics, CancellationToken? cancelToken = null);

        Task<DogMetricQueryResult> QueryAsync(DateTimeOffset from, DateTimeOffset to, string query,
            CancellationToken? cancelToken = null);
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
        [DataMember(Name = "series")]
        public DogMetric[] Series { get; set; }
    }

    public class DogMetricPostResult
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
    }

    public class DogMetric
    {
        public DogMetric(string metric, DogMetricPoint[] points)
        {
            Metric = metric;
            Points = points;
        }

        public DogMetric(string metric, DateTimeOffset timestamp, double value) : this(metric,
            new[] {new DogMetricPoint(timestamp, value)})
        {
            Metric = metric;
            Points = new[] {new DogMetricPoint(timestamp, value)};
        }

        public DogMetric(string metric, double value)
        {
            Metric = metric;
            Points = new[] {new DogMetricPoint(DateTimeOffset.Now, value)};
        }

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
    }

    [JsonFormatter(typeof(DogMetricPointFormatter))]
    public struct DogMetricPoint
    {
        public DateTimeOffset Timestamp { get; set; }
        public double Value { get; set; }

        public DogMetricPoint(DateTimeOffset timestamp, double value)
        {
            Timestamp = timestamp;
            Value = value;
        }
    }

    public class DogMetricQueryResult
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "res_type")]
        public string ResourceType { get; set; }

        [DataMember(Name = "series")]
        public DogMetricQueryResultSeries[] Series { get; set; }

        [DataMember(Name = "from_date")]
        [JsonFormatter(typeof(UnixTimeMillisecondsDateTimeOffsetFormatter))]
        public DateTimeOffset FromDate { get; set; }

        [DataMember(Name = "to_date")]
        [JsonFormatter(typeof(UnixTimeMillisecondsDateTimeOffsetFormatter))]
        public DateTimeOffset ToDate { get; set; }

        [DataMember(Name = "group_by")]
        public string[] GroupBy { get; set; }

        [DataMember(Name = "resp_version")]
        public int ResponseVersion { get; set; }

        [DataMember(Name = "query")]
        public string Query { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }
    }

    public class DogMetricQueryResultSeries
    {
        [DataMember(Name = "metric")]
        public string Metric { get; set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "unit")]
        public string[] Unit { get; set; }

        [DataMember(Name = "pointlist")]
        public DogMetricPoint[] Points { get; set; }

        [DataMember(Name = "end")]
        [JsonFormatter(typeof(UnixTimeMillisecondsDateTimeOffsetFormatter))]
        public DateTimeOffset End { get; set; }

        [DataMember(Name = "interval")]
        public int Interval { get; set; }

        [DataMember(Name = "start")]
        [JsonFormatter(typeof(UnixTimeMillisecondsDateTimeOffsetFormatter))]
        public DateTimeOffset Start { get; set; }

        [DataMember(Name = "length")]
        public int Length { get; set; }

        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "expression")]
        public string Expression { get; set; }
    }

    partial class DogApiClient : IMetricApi
    {
        public IMetricApi Metric => this;

        async Task<DogMetricGetListResult> IMetricApi.GetListAsync(DateTimeOffset from, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection
            {
                {"from", from.ToUnixTimeSeconds().ToString()}
            };
            return await RequestAsync<DogMetricGetListResult>(HttpMethod.Get, $"/api/v1/metrics", @params, null,
                cancelToken).ConfigureAwait(false);
        }

        async Task<DogMetricPostResult> IMetricApi.PostAsync(DogMetric[] metrics, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogMetricPostParameter {Series = metrics}, StandardResolver.ExcludeNull));
            return await RequestAsync<DogMetricPostResult>(HttpMethod.Post, "/api/v1/series", null, data, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogMetricQueryResult> IMetricApi.QueryAsync(DateTimeOffset from, DateTimeOffset to, string query,
            CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection
            {
                {"from", from.ToUnixTimeSeconds().ToString()},
                {"to", to.ToUnixTimeSeconds().ToString()},
                {"query", query}
            };
            return await RequestAsync<DogMetricQueryResult>(HttpMethod.Get, $"/api/v1/query", @params, null,
                cancelToken).ConfigureAwait(false);
        }
    }
}