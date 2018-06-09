using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DogApiNet
{
    public interface IEmbedApi
    {
        Task<DogEmbedGraph[]> GetAllAsync(CancellationToken? cancelToken = null);

        Task<DogEmbedGraph> GetAsync(string id, CancellationToken? cancelToken = null);

        Task<DogEmbedGraph> CreateAsync(DogEmbedCreateParameter param, CancellationToken? cancelToken = null);

        Task EnableAsync(string id, CancellationToken? cancelToken = null);

        Task RevokeAsync(string id, CancellationToken? cancelToken = null);
    }

    public class DogEmbedCreateParameter
    {
        public DogEmbedCreateParameter(string graphJson)
        {
            GraphJson = graphJson;
        }

        public string GraphJson { get; set; }
        public string Timeframe { get; set; }
        public string Size { get; set; }
        public bool? Legend { get; set; }
        public string Title { get; set; }
    }

    public class DogEmbedGraph
    {
        [DataMember(Name = "embed_id")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "template_variables")]
        public DogTemplateVariable[] TemplateVariables { get; set; }

        [DataMember(Name = "html")]
        public string Html { get; set; }

        [DataMember(Name = "revoked")]
        public bool Revoked { get; set; }

        [DataMember(Name = "dash_url")]
        public string DashUrl { get; set; }

        [DataMember(Name = "dash_name")]
        public string DashName { get; set; }

        [DataMember(Name = "shared_by")]
        public long SharedBy { get; set; }
    }

    public class DogEmbedGetAllResult
    {
        [DataMember(Name = "embedded_graphs")]
        public DogEmbedGraph[] EmbeddedGraphs { get; set; }
    }

    partial class DogApiClient : IEmbedApi
    {
        public IEmbedApi Embed => this;

        async Task<DogEmbedGraph> IEmbedApi.CreateAsync(DogEmbedCreateParameter param, CancellationToken? cancelToken)
        {
            var sb = new StringBuilder();
            sb.Append("graph_json=").Append(WebUtility.UrlEncode(param.GraphJson));
            if (!string.IsNullOrEmpty(param.Timeframe))
                sb.Append("&timeframe=").Append(WebUtility.UrlEncode(param.Timeframe));
            if (!string.IsNullOrEmpty(param.Size)) sb.Append("&size=").Append(WebUtility.UrlEncode(param.Size));
            if (param.Legend.HasValue) sb.Append("&legend=").Append(param.Legend.Value ? "yes" : "no");
            if (!string.IsNullOrEmpty(param.Title)) sb.Append("&title=").Append(WebUtility.UrlEncode(param.Title));

            var data = new DogApiHttpRequestContent("application/x-www-form-urlencoded",
                Encoding.UTF8.GetBytes(sb.ToString()));
            return await RequestAsync<DogEmbedGraph>(HttpMethod.Post, $"/api/v1/graph/embed", null, data, cancelToken)
                .ConfigureAwait(false);
        }

        async Task IEmbedApi.EnableAsync(string id, CancellationToken? cancelToken) =>
            await RequestAsync<NoJsonResponse>(HttpMethod.Get, $"/api/v1/graph/embed/{id}/enable", null, null,
                cancelToken).ConfigureAwait(false);

        async Task<DogEmbedGraph[]> IEmbedApi.GetAllAsync(CancellationToken? cancelToken) =>
            (await RequestAsync<DogEmbedGetAllResult>(HttpMethod.Get, $"/api/v1/graph/embed", null, null, cancelToken).ConfigureAwait(false)).EmbeddedGraphs;

        async Task<DogEmbedGraph> IEmbedApi.GetAsync(string id, CancellationToken? cancelToken) =>
            await RequestAsync<DogEmbedGraph>(HttpMethod.Get, $"/api/v1/graph/embed/{id}", null, null, cancelToken)
                .ConfigureAwait(false);

        async Task IEmbedApi.RevokeAsync(string id, CancellationToken? cancelToken) =>
            await RequestAsync<DogEmbedGraph>(HttpMethod.Get, $"/api/v1/graph/embed/{id}/revoke", null, null,
                cancelToken).ConfigureAwait(false);
    }
}