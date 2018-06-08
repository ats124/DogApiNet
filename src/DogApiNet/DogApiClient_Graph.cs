using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DogApiNet
{
    public interface IGraphApi
    {
        Task<DogGraphSnapshot> CreateAsync(string metricQuery, DateTimeOffset start, DateTimeOffset end,
            string eventQuery = null, string title = null, CancellationToken? cancelToken = null);

        Task<DogGraphSnapshot> CreateByGraphDefAsync(string graphDef, DateTimeOffset start, DateTimeOffset end, string title = null, CancellationToken? cancelToken = null);
    }

    public class DogGraphSnapshot
    {
        [DataMember(Name = "graph_def")]
        public string GraphDefinition { get; set; }

        [DataMember(Name = "snapshot_url")]
        public string SnapshotUrl { get; set; }

        [DataMember(Name = "metric_query")]
        public string MetricQuery { get; set; }

        [DataMember(Name = "event_query")]
        public string EventQuery { get; set; }
    }

    partial class DogApiClient : IGraphApi
    {
        public IGraphApi Graph => this;

        Task<DogGraphSnapshot> IGraphApi.CreateAsync(string metricQuery, DateTimeOffset start, DateTimeOffset end,
            string eventQuery, string title, CancellationToken? cancelToken) =>
            GraphCreateAsync(metricQuery, null, start, end, eventQuery, title, cancelToken);

        Task<DogGraphSnapshot> IGraphApi.CreateByGraphDefAsync(string graphDef, DateTimeOffset start,
            DateTimeOffset end, string title, CancellationToken? cancelToken) =>
            GraphCreateAsync(null, graphDef, start, end, null, title, cancelToken);

        private async Task<DogGraphSnapshot> GraphCreateAsync(string metricQuery, string graphDef, DateTimeOffset start,
            DateTimeOffset end, string eventQuery, string title, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (metricQuery != null) @params["metric_query"] = metricQuery;
            if (graphDef != null) @params["graph_def"] = graphDef;
            @params["start"] = start.ToUnixTimeSeconds().ToString();
            @params["end"] = end.ToUnixTimeSeconds().ToString();
            if (eventQuery != null) @params["event_query"] = eventQuery;
            if (title != null) @params["title"] = title;

            return await RequestAsync<DogGraphSnapshot>(HttpMethod.Get, $"/api/v1/graph/snapshot", @params, null,
                cancelToken).ConfigureAwait(false);
        }
    }
}