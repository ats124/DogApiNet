using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DogApiNet
{
    public interface ITagApi
    {
        Task<ILookup<string, string>> GetAllAsync(string source = null, CancellationToken? cancelToken = null);
    }

    public class DogTagGetAllResult
    {
        [DataMember(Name = "tags")]
        public IDictionary<string, string[]> Tags { get; set; }
    }

    partial class DogApiClient : ITagApi
    {
        public ITagApi Tag => this;

        async Task<ILookup<string, string>> ITagApi.GetAllAsync(string source, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (source != null) @params["source"] = source;
            var result =
                await RequestAsync<DogTagGetAllResult>(HttpMethod.Get, $"/api/v1/tags/hosts", @params, null,
                        cancelToken)
                    .ConfigureAwait(false);
            if (result?.Tags == null)
                return new string[0].ToLookup(x => x);
            return result.Tags
                .SelectMany(x => x.Value.Select(y => (key: x.Key, value: y)))
                .ToLookup(x => x.key, x => x.value);
        }
    }
}