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

    public interface IMetadataApi
    {
        Task<DogMetadataGetResult> GetAsync(string metric, CancellationToken? cancelToken = null);

        Task<DogMetadataUpdateResult> UpdateAsync(DogMetadataUpdateParameter param, CancellationToken? cancelToken = null);
    }

    public class DogMetadataGetResult
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "short_name")]
        public string ShortName { get; set; }

        [DataMember(Name = "statsd_interval")]
        public int? StatsDInterval { get; set; }

        [DataMember(Name = "per_unit")]
        public string PerUnit { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "unit")]
        public string Unit { get; set; }
    }

    public class DogMetadataUpdateParameter
    {
        [IgnoreDataMember]
        public string Metric { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "short_name")]
        public string ShortName { get; set; }

        [DataMember(Name = "statsd_interval")]
        public int? StatsDInterval { get; set; }

        [DataMember(Name = "per_unit")]
        public string PerUnit { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "unit")]
        public string Unit { get; set; }

        public DogMetadataUpdateParameter(string metric)
        {
            Metric = metric;
        }
    }

    public class DogMetadataUpdateResult
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "short_name")]
        public string ShortName { get; set; }

        [DataMember(Name = "statsd_interval")]
        public int? StatsDInterval { get; set; }

        [DataMember(Name = "per_unit")]
        public string PerUnit { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "unit")]
        public string Unit { get; set; }
    }

    partial class DogApiClient : IMetadataApi
    {
        public IMetadataApi Metadata => this;

        async Task<DogMetadataGetResult> IMetadataApi.GetAsync(string metric, CancellationToken? cancelToken)
        {
            return await RequestAsync<DogMetadataGetResult>(HttpMethod.Get, $"/api/v1/metrics/{metric}", null, null, cancelToken).ConfigureAwait(false);
        }

        async Task<DogMetadataUpdateResult> IMetadataApi.UpdateAsync(DogMetadataUpdateParameter param, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(param));
            return await RequestAsync<DogMetadataUpdateResult>(HttpMethod.Put, $"/api/v1/metrics/{param.Metric}", null, data, cancelToken).ConfigureAwait(false);
        }
    }
}
