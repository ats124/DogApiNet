using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace DogApiNet
{
    public interface IMetadataApi
    {
        Task<DogMetadata> GetAsync(string metric, CancellationToken? cancelToken = null);

        Task<DogMetadata> UpdateAsync(DogMetadata metadata, CancellationToken? cancelToken = null);
    }

    public class DogMetadata
    {
        public DogMetadata()
        {
        }

        public DogMetadata(string metric)
        {
            Metric = metric;
        }

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
    }

    partial class DogApiClient : IMetadataApi
    {
        public IMetadataApi Metadata => this;

        async Task<DogMetadata> IMetadataApi.GetAsync(string metric, CancellationToken? cancelToken)
        {
            var metadata =
                await RequestAsync<DogMetadata>(HttpMethod.Get, $"/api/v1/metrics/{metric}", null, null, cancelToken)
                    .ConfigureAwait(false);
            metadata.Metric = metric;
            return metadata;
        }

        async Task<DogMetadata> IMetadataApi.UpdateAsync(DogMetadata metadata, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(metadata));
            var updatedMetadata =
                await RequestAsync<DogMetadata>(HttpMethod.Put, $"/api/v1/metrics/{metadata.Metric}", null, data,
                    cancelToken).ConfigureAwait(false);
            updatedMetadata.Metric = metadata.Metric;
            return updatedMetadata;
        }
    }
}