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
    }

    public class DogMetricGetListResult
    {
        [DataMember(Name = "metrics")]
        public string[] Metrics { get; set; }

        [DataMember(Name = "from")]
        [JsonFormatter(typeof(DurableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset From { get; set; }
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
    }

}
