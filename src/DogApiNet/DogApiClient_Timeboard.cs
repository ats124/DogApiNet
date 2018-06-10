using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DogApiNet.Internal;
using DogApiNet.JsonFormatters;
using Utf8Json;

namespace DogApiNet
{
    public interface ITimeboardApi
    {
        Task<DogTimeboard> CreateAsync(DogTimeboard timeboard, CancellationToken? cancelToken = null);

        Task<DogTimeboard> UpdateAsync(DogTimeboard timeboard, CancellationToken? cancelToken = null);

        Task DeleteAsync(long id, CancellationToken? cancelToken = null);

        Task<DogTimeboard> GetAsync(long id, CancellationToken? cancelToken = null);

        Task<DogTimeboardSummary[]> GetAllAsync(CancellationToken? cancelToken = null);
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

        [DataMember(Name = "template_variables")]
        public DogTemplateVariable[] TemplateVariables { get; set; }

        [DataMember(Name = "read_only")]
        public bool? ReadOnly { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset? Modified { get; set; }

        [IgnoreDataMember]
        public string Resource { get; set; }

        [IgnoreDataMember]
        public string Url { get; set; }

        public bool ShouldSerializeId() => false;

        public bool ShouldSerializeTemplateVariables() => TemplateVariables != null;

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

    namespace Internal
    {
        public class DogTimeboardGetAllResult
        {
            [DataMember(Name = "dashes")]
            public DogTimeboardSummary[] Timeboards { get; set; }
        }

        public class DogTimeboardResult
        {
            [DataMember(Name = "dash")]
            public DogTimeboard Timeboard { get; set; }

            [DataMember(Name = "resource")]
            public string Resource { get; set; }

            [DataMember(Name = "url")]
            public string Url { get; set; }
        }
    }

    partial class DogApiClient : ITimeboardApi
    {
        public ITimeboardApi Timeboard => this;

        async Task ITimeboardApi.DeleteAsync(long id, CancellationToken? cancelToken)
        {
            await RequestAsync<NoJsonResponse>(HttpMethod.Delete, $"/api/v1/dash/{id}", null,
                DogApiHttpRequestContent.EmptyJson, cancelToken).ConfigureAwait(false);
        }

        async Task<DogTimeboard> ITimeboardApi.CreateAsync(DogTimeboard timeboard,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(timeboard));
            var result = await RequestAsync<DogTimeboardResult>(HttpMethod.Post, "/api/v1/dash", null, data,
                cancelToken).ConfigureAwait(false);
            if (result.Timeboard != null)
            {
                result.Timeboard.Url = result.Url;
                result.Timeboard.Resource = result.Resource;
            }

            return result.Timeboard;
        }

        async Task<DogTimeboard> ITimeboardApi.UpdateAsync(DogTimeboard timeboard,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(timeboard));
            var result = await RequestAsync<DogTimeboardResult>(HttpMethod.Put, $"/api/v1/dash/{timeboard.Id}",
                null,
                data, cancelToken).ConfigureAwait(false);
            if (result.Timeboard != null)
            {
                result.Timeboard.Url = result.Url;
                result.Timeboard.Resource = result.Resource;
            }

            return result.Timeboard;
        }

        async Task<DogTimeboard> ITimeboardApi.GetAsync(long id, CancellationToken? cancelToken)
        {
            var result =
                await RequestAsync<DogTimeboardResult>(HttpMethod.Get, $"/api/v1/dash/{id}", null, null, cancelToken)
                    .ConfigureAwait(false);
            if (result.Timeboard != null)
            {
                result.Timeboard.Url = result.Url;
                result.Timeboard.Resource = result.Resource;
            }

            return result.Timeboard;
        }

        async Task<DogTimeboardSummary[]> ITimeboardApi.GetAllAsync(CancellationToken? cancelToken)
        {
            var result =
                await RequestAsync<DogTimeboardGetAllResult>(HttpMethod.Get, $"/api/v1/dash", null, null, cancelToken)
                    .ConfigureAwait(false);
            return result.Timeboards;
        }
    }
}