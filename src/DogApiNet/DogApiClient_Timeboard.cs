using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DogApiNet.JsonFormatters;
using Utf8Json;

namespace DogApiNet
{
    public interface ITimeboardApi
    {
        Task<DogTimeboardCreateResult> CreateAsync(DogTimeboard timeboard, CancellationToken? cancelToken = null);

        Task<DogTimeboardUpdateResult> UpdateAsync(DogTimeboard timeboard, CancellationToken? cancelToken = null);

        Task DeleteAsync(long id, CancellationToken? cancelToken = null);

        Task<DogTimeboardGetResult> GetAsync(long id, CancellationToken? cancelToken = null);

        Task<DogTimeboardGetAllResult> GetAllAsync(CancellationToken? cancelToken = null);
    }

    public class DogTimeboardGetResult
    {
        [DataMember(Name = "dash")]
        public DogTimeboard Timeboard { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    public class DogTimeboardCreateResult : DogTimeboardGetResult
    {
    }

    public class DogTimeboardUpdateResult : DogTimeboardGetResult
    {
    }

    public class DogTimeboardGetAllResult
    {
        [DataMember(Name = "dashes")]
        public DogTimeboardSummary[] Timeboards { get; set; }
    }

    public class DogTimeboard
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "graphs")]
        public DogTimeboardGraph[] Graphs { get; set; }

        [DataMember(Name = "read_only")]
        public bool? ReadOnly { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset? Modified { get; set; }

        public bool ShouldSerializeId() => false;

        public bool ShouldSerializeReadOnly() => ReadOnly.HasValue;

        public bool ShouldSerializeCreated() => false;

        public bool ShouldSerializeModified() => false;
    }

    public class DogTimeboardGraph
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "definition")]
        public dynamic Definition { get; set; }

        public string GetDefinitionJsonString() => JsonSerializer.ToJsonString((object)Definition);

        public void SetDefinitionJsonString(string json) => Definition = JsonSerializer.Deserialize<dynamic>(json);
    }

    public class DogTimeboardTemplateVariable
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "prefix")]
        public string Prefix { get; set; }

        [DataMember(Name = "default")]
        public string Default { get; set; }
    }

    public class DogTimeboardSummary
    {
        [DataMember(Name = "id")]
        [JsonFormatter(typeof(DurableInt64Formatter))]
        public long Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset? Modified { get; set; }

        [DataMember(Name = "read_only")]
        [JsonFormatter(typeof(DurableBooleanFormatter), true)]
        public bool ReadOnly { get; set; }
    }

    partial class DogApiClient : ITimeboardApi
    {
        public ITimeboardApi Timeboard => this;

        async Task ITimeboardApi.DeleteAsync(long id, CancellationToken? cancelToken)
        {
            await RequestAsync<NoJsonResponse>(HttpMethod.Delete, $"/api/v1/dash/{id}", null,
                DogApiHttpRequestContent.EmptyJson, cancelToken).ConfigureAwait(false);
        }

        async Task<DogTimeboardCreateResult> ITimeboardApi.CreateAsync(DogTimeboard timeboard,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(timeboard));
            return await RequestAsync<DogTimeboardCreateResult>(HttpMethod.Post, "/api/v1/dash", null, data,
                cancelToken).ConfigureAwait(false);
        }

        async Task<DogTimeboardUpdateResult> ITimeboardApi.UpdateAsync(DogTimeboard timeboard,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(timeboard));
            return await RequestAsync<DogTimeboardUpdateResult>(HttpMethod.Put, $"/api/v1/dash/{timeboard.Id}", null,
                data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogTimeboardGetResult> ITimeboardApi.GetAsync(long id, CancellationToken? cancelToken) =>
            await RequestAsync<DogTimeboardGetResult>(HttpMethod.Get, $"/api/v1/dash/{id}", null, null, cancelToken)
                .ConfigureAwait(false);

        async Task<DogTimeboardGetAllResult> ITimeboardApi.GetAllAsync(CancellationToken? cancelToken) =>
            await RequestAsync<DogTimeboardGetAllResult>(HttpMethod.Get, $"/api/v1/dash", null, null, cancelToken)
                .ConfigureAwait(false);
    }
}