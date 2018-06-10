using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DogApiNet.Internal;

namespace DogApiNet
{
    public interface IInfrastructureApi
    {
        Task<IDictionary<string, string[]>> SearchAsync(string q, CancellationToken? cancelToken = null);
    }

    namespace Internal
    {
        public class DogInfrastructureSearchResult
        {
            [DataMember(Name = "results")]
            public IDictionary<string, string[]> Results { get; set; }
        }
    }

    partial class DogApiClient : IInfrastructureApi
    {
        public IInfrastructureApi Infrastructure => this;

        async Task<IDictionary<string, string[]>> IInfrastructureApi.SearchAsync(string q,
            CancellationToken? cancelToken) =>
            (await RequestAsync<DogInfrastructureSearchResult>(HttpMethod.Get, "/api/v1/search",
                    new NameValueCollection {{"q", q}}, null, cancelToken)
                .ConfigureAwait(false)).Results;
    }
}