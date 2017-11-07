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

    public interface IServiceCheckApi
    {
        Task<DogServiceCheckPostResult> PostAsync(DogServiceCheckPostParameter param, CancellationToken? cancelToken = null);
    }

    public class DogServiceCheckPostParameter
    {
        [DataMember(Name = "check")]
        public string Check { get; set; }

        [DataMember(Name = "host_name")]
        public string HostName { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "timestamp")]
        [JsonFormatter(typeof(NullableUnixTimeSecondsDateTimeOffsetFormatter))]
        public DateTimeOffset? Timestamp { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "tags")]
        public string[] Tags { get; set; }

        public DogServiceCheckPostParameter(string check, string hostName, int status)
        {
            Check = check;
            HostName = hostName;
            Status = status;
        }
    }

    public class DogServiceCheckPostResult
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
    }

    partial class DogApiClient : IServiceCheckApi
    {
        public IServiceCheckApi ServiceCheck => this;

        async Task<DogServiceCheckPostResult> IServiceCheckApi.PostAsync(DogServiceCheckPostParameter param, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(param, Utf8Json.Resolvers.StandardResolver.ExcludeNull));
            return await RequestAsync<DogServiceCheckPostResult>(HttpMethod.Post, "/api/v1/check_run", null, data, cancelToken);
        }

    }
}
