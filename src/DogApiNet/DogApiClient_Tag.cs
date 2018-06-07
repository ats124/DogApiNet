using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace DogApiNet
{
    public interface ITagApi
    {
        Task<ILookup<string, string>> GetAllAsync(string source = null, CancellationToken? cancelToken = null);

        Task<string[]> GetAsync(string hostName, string source = null, CancellationToken? cancelToken = null);

        Task<ILookup<string, string>> GetBySourceAsync(string hostName, string source = null,
            CancellationToken? cancelToken = null);

        Task<string[]> CreateAsync(string hostName, string[] tags, string source = null, CancellationToken? cancelToken = null);

        Task<string[]> UpdateAsync(string hostName, string[] tags, string source = null, CancellationToken? cancelToken = null);
    }

    public class DogTagGetAllResult
    {
        [DataMember(Name = "tags")]
        public IDictionary<string, string[]> Tags { get; set; }
    }

    public class DogTagGetResult
    {
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
    }

    public class DogTagGetBySourceResult
    {
        [DataMember(Name = "tags")]
        public IDictionary<string, string[]> Tags { get; set; }
    }

    public class DogTagCreateUpdateParameter
    {
        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
    }

    public class DogTagCreateUpdateResult
    {
        [DataMember(Name = "hostname")]
        public string[] HostName { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }
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

        async Task<string[]> ITagApi.GetAsync(string hostName, string source, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (source != null) @params["source"] = source;
            var result =
                await RequestAsync<DogTagGetResult>(HttpMethod.Get,
                        $"/api/v1/tags/hosts/{Uri.EscapeDataString(hostName)}", @params, null,
                        cancelToken)
                    .ConfigureAwait(false);
            return result?.Tags ?? new string[0];
        }

        async Task<ILookup<string, string>> ITagApi.GetBySourceAsync(string hostName, string source,
            CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (source != null) @params["source"] = source;
            @params["by_source"] = "true";
            var result =
                await RequestAsync<DogTagGetBySourceResult>(HttpMethod.Get,
                        $"/api/v1/tags/hosts/{Uri.EscapeDataString(hostName)}", @params, null,
                        cancelToken)
                    .ConfigureAwait(false);
            if (result?.Tags == null)
                return new string[0].ToLookup(x => x);
            return result.Tags
                .SelectMany(x => x.Value.Select(y => (key: x.Key, value: y)))
                .ToLookup(x => x.key, x => x.value);
        }

        async Task<string[]> ITagApi.CreateAsync(string hostName, string[] tags, string source, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (source != null) @params["source"] = source;

            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogTagCreateUpdateParameter {Tags = tags}));

            var result =
                await RequestAsync<DogTagCreateUpdateResult>(HttpMethod.Post,
                        $"/api/v1/tags/hosts/{Uri.EscapeDataString(hostName)}", @params, data,
                        cancelToken)
                    .ConfigureAwait(false);
            return result?.Tags ?? new string[0];
        }

        async Task<string[]> ITagApi.UpdateAsync(string hostName, string[] tags, string source, CancellationToken? cancelToken)
        {
            var @params = new NameValueCollection();
            if (source != null) @params["source"] = source;

            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogTagCreateUpdateParameter {Tags = tags}));

            var result =
                await RequestAsync<DogTagCreateUpdateResult>(HttpMethod.Put,
                        $"/api/v1/tags/hosts/{Uri.EscapeDataString(hostName)}", @params, data,
                        cancelToken)
                    .ConfigureAwait(false);
            return result?.Tags ?? new string[0];
        }
    }
}