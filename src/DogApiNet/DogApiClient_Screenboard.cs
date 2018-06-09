using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace DogApiNet
{
    public interface IScreenboardApi
    {
        Task<DogScreenboard> CreateAsync(DogScreenboard screenBoard, CancellationToken? cancelToken = null);

        Task<DogScreenboard> UpdateAsync(DogScreenboard screenBoard, CancellationToken? cancelToken = null);

        Task DeleteAsync(long id, CancellationToken? cancelToken = null);

        Task<DogScreenboard> GetAsync(long id, CancellationToken? cancelToken = null);

        Task<DogScreenboardSummary[]> GetAllAsync(CancellationToken? cancelToken = null);

        Task<DogScreenboardShareResult> ShareAsync(long id, CancellationToken? cancelToken = null);

        Task RevokeAsync(long id, CancellationToken? cancelToken = null);
    }

    public class DogScreenboard
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "board_title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "width")]
        public dynamic Width { get; set; }

        [DataMember(Name = "height")]
        public dynamic Height { get; set; }

        [DataMember(Name = "widgets")]
        public dynamic Widgets { get; set; }

        [DataMember(Name = "template_variables")]
        public DogTemplateVariable[] TemplateVariables { get; set; }

        [DataMember(Name = "read_only")]
        public bool? ReadOnly { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset? Modified { get; set; }

        public string GetWidgetsJsonString() => JsonSerializer.ToJsonString((object)Widgets);

        public void SetWidgetsJsonString(string json) => Widgets = JsonSerializer.Deserialize<dynamic>(json);

        public bool ShouldSerializeId() => false;

        public bool ShouldSerializeTemplateVariables() => TemplateVariables != null;

        public bool ShouldSerializeReadOnly() => ReadOnly.HasValue;

        public bool ShouldSerializeCreated() => false;

        public bool ShouldSerializeModified() => false;
    }

    public class DogScreenboardSummary
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "read_only")]
        public bool? ReadOnly { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset? Modified { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }
    }

    public class DogScreenboardGetAllResult
    {
        [DataMember(Name = "screenboards")]
        public DogScreenboardSummary[] Screenboards { get; set; }
    }

    public class DogScreenboardShareResult
    {
        [DataMember(Name = "board_id")]
        public long Id { get; set; }

        [DataMember(Name = "public_url")]
        public string PublicUrl { get; set; }
    }

    partial class DogApiClient : IScreenboardApi
    {
        public IScreenboardApi Screenboard => this;

        async Task<DogScreenboard> IScreenboardApi.CreateAsync(DogScreenboard screenBoard,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(screenBoard));
            return await RequestAsync<DogScreenboard>(HttpMethod.Post, "/api/v1/screen", null, data,
                cancelToken).ConfigureAwait(false);
        }

        async Task<DogScreenboard> IScreenboardApi.UpdateAsync(DogScreenboard screenBoard,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(screenBoard));
            return await RequestAsync<DogScreenboard>(HttpMethod.Put, $"/api/v1/screen/{screenBoard.Id}", null, data,
                cancelToken).ConfigureAwait(false);
        }

        async Task IScreenboardApi.DeleteAsync(long id, CancellationToken? cancelToken)
        {
            await RequestAsync<NoJsonResponse>(HttpMethod.Delete, $"/api/v1/screen/{id}", null,
                DogApiHttpRequestContent.EmptyJson, cancelToken).ConfigureAwait(false);
        }

        async Task<DogScreenboard> IScreenboardApi.GetAsync(long id, CancellationToken? cancelToken) =>
            await RequestAsync<DogScreenboard>(HttpMethod.Get, $"/api/v1/screen/{id}", null,
                null, cancelToken).ConfigureAwait(false);

        async Task<DogScreenboardSummary[]> IScreenboardApi.GetAllAsync(CancellationToken? cancelToken)
        {
            var result = await RequestAsync<DogScreenboardGetAllResult>(HttpMethod.Get, $"/api/v1/screen", null,
                null, cancelToken).ConfigureAwait(false);
            return result.Screenboards;
        }

        async Task<DogScreenboardShareResult> IScreenboardApi.ShareAsync(long id, CancellationToken? cancelToken) =>
            await RequestAsync<DogScreenboardShareResult>(HttpMethod.Post, $"/api/v1/screen/share/{id}", null,
                DogApiHttpRequestContent.EmptyJson, cancelToken).ConfigureAwait(false);

        async Task IScreenboardApi.RevokeAsync(long id, CancellationToken? cancelToken)
        {
            await RequestAsync<DogScreenboardShareResult>(HttpMethod.Delete, $"/api/v1/screen/share/{id}", null,
                DogApiHttpRequestContent.EmptyJson, cancelToken).ConfigureAwait(false);
        }
    }
}