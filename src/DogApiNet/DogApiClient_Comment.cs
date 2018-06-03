using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DogApiNet.JsonFormatters;
using Utf8Json;

namespace DogApiNet
{
    public interface ICommentApi
    {
        Task<DogComment> CreateAsync(DogComment comment, CancellationToken? cancelToken = null);
        Task<DogComment> UpdateAsync(DogComment comment, CancellationToken? cancelToken = null);
        Task DeleteAsync(long id, CancellationToken? cancelToken = null);
    }

    public class DogComment
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "handle")]
        public string Handle { get; set; }

        [DataMember(Name = "related_event_id")]
        [JsonFormatter(typeof(DurableNullableInt64Formatter))]
        public long? RelatedEventId { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        public bool ShouldSerializeId() => false;
        public bool ShouldSerializeResource() => false;
        public bool ShouldSerializeUrl() => false;
    }

    public class DogCommentCreateUpdateResult
    {
        [DataMember(Name = "comment")]
        public DogComment Comment { get; set; }
    }

    partial class DogApiClient : ICommentApi
    {
        public ICommentApi Comment => this;

        async Task<DogComment> ICommentApi.CreateAsync(DogComment comment, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(comment));
            var result =
                await RequestAsync<DogCommentCreateUpdateResult>(HttpMethod.Post, "/api/v1/comments", null, data,
                        cancelToken)
                    .ConfigureAwait(false);
            return result.Comment;
        }

        async Task ICommentApi.DeleteAsync(long id, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", new byte[0]);
            await RequestAsync<NoJsonResponse>(HttpMethod.Delete, $"/api/v1/comments/{id}", null, data, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogComment> ICommentApi.UpdateAsync(DogComment comment, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(comment));
            var result = await RequestAsync<DogCommentCreateUpdateResult>(HttpMethod.Put,
                $"/api/v1/comments/{comment.Id}", null, data,
                cancelToken).ConfigureAwait(false);
            return result.Comment;
        }
    }
}