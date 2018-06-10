using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public interface IHostApi
    {
        Task<DogHostSearchResult> SearchAsync(string filter = null, string sortField = null, string sortDir = null,
            int? start = null, int? count = null, CancellationToken? cancelToken = null);

        Task<DogHostTotals> TotalsAsync(CancellationToken? cancelToken = null);

        Task<DogHostMuteResult> MuteAsync(string hostName, DateTimeOffset? end = null, string message = null,
            bool? @override = null, CancellationToken? cancelToken = null);

        Task<DogHostUnmuteResult> UnmuteAsync(string hostName, CancellationToken? cancelToken = null);
    }

    public class DogHost
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "is_muted")]
        public bool IsMuted { get; set; }

        [DataMember(Name = "apps")]
        public string[] Apps { get; set; }

        [DataMember(Name = "tags_by_source")]
        public IDictionary<string, string[]> TagsBySource { get; set; }

        [DataMember(Name = "up")]
        public bool? Up { get; set; }

        [DataMember(Name = "metrics")]
        public DogHostMetrics Metrics { get; set; }

        [DataMember(Name = "sources")]
        public string[] Sources { get; set; }

        [DataMember(Name = "meta")]
        public IDictionary<string, object> Meta { get; set; }

        [DataMember(Name = "host_name")]
        public string HostName { get; set; }

        [DataMember(Name = "aliases")]
        public string[] Aliases { get; set; }
    }

    public class DogHostMetrics
    {
        [DataMember(Name = "load")]
        public double? Load { get; set; }

        [DataMember(Name = "iowait")]
        public double? IoWait { get; set; }

        [DataMember(Name = "cpu")]
        public double? Cpu { get; set; }
    }

    public class DogHostSearchResult
    {
        [DataMember(Name = "total_returned")]
        public int TotalReturned { get; set; }

        [DataMember(Name = "total_matching")]
        public int TotalMatching { get; set; }

        [DataMember(Name = "host_list")]
        public DogHost[] Hosts { get; set; }
    }

    public class DogHostTotals
    {
        [DataMember(Name = "total_up")]
        public int TotalUp { get; set; }

        [DataMember(Name = "total_active")]
        public int TotalActive { get; set; }
    }

    public class DogHostMuteResult
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "hostname")]
        public string HostName { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "end")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? End { get; set; }
    }

    public class DogHostUnmuteResult
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [DataMember(Name = "hostname")]
        public string HostName { get; set; }
    }

    namespace Internal
    {
        public class DogHostMuteParameter
        {
            [DataMember(Name = "end")]
            [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
            public DateTimeOffset? End { get; set; }

            [DataMember(Name = "message")]
            public string Message { get; set; }

            [DataMember(Name = "override")]
            public bool? Override { get; set; }
        }
    }

    partial class DogApiClient : IHostApi
    {
        public IHostApi Host => this;

        async Task<DogHostSearchResult> IHostApi.SearchAsync(string filter, string sortField, string sortDir,
            int? start,
            int? count, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (!string.IsNullOrEmpty(filter)) @params["filter"] = filter;
            if (!string.IsNullOrEmpty(sortField)) @params["sort_field"] = sortField;
            if (!string.IsNullOrEmpty(sortDir)) @params["sort_dir"] = sortDir;
            if (start.HasValue) @params["start"] = start.Value.ToString();
            if (count.HasValue) @params["count"] = count.Value.ToString();

            return await RequestAsync<DogHostSearchResult>(HttpMethod.Get, "/api/v1/hosts", @params, null, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogHostTotals> IHostApi.TotalsAsync(CancellationToken? cancelToken) =>
            await RequestAsync<DogHostTotals>(HttpMethod.Get, "/api/v1/hosts/totals", null, null, cancelToken)
                .ConfigureAwait(false);

        async Task<DogHostMuteResult> IHostApi.MuteAsync(string hostName, DateTimeOffset? end, string message,
            bool? @override, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(
                    new DogHostMuteParameter {End = end, Message = message, Override = @override},
                    StandardResolver.ExcludeNull));
            return await RequestAsync<DogHostMuteResult>(HttpMethod.Post, $"/api/v1/host/{hostName}/mute", null,
                    data, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogHostUnmuteResult> IHostApi.UnmuteAsync(string hostName, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", new byte[0]);
            return await RequestAsync<DogHostUnmuteResult>(HttpMethod.Post, $"/api/v1/host/{hostName}/unmute", null,
                    data, cancelToken)
                .ConfigureAwait(false);
        }
    }
}