using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DogApiNet.Internal;
using Utf8Json;

namespace DogApiNet
{
    public interface IUserApi
    {
        Task<DogUser> CreateAsync(string handle, string name = null, string accessRole = null,
            CancellationToken? cancelToken = null);

        Task<DogUser> GetAsync(string handle, CancellationToken? cancelToken = null);

        Task<DogUser[]> GetAllAsync(CancellationToken? cancelToken = null);

        Task<DogUser> UpdateAsync(DogUser user, CancellationToken? cancelToken = null);

        Task DeleteAsync(string handle, CancellationToken? cancelToken = null);
    }

    public class DogUser
    {
        [DataMember(Name = "handle")]
        public string Handle { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "access_role")]
        public string AccessRole { get; set; }

        [DataMember(Name = "email")]
        public string EMail { get; set; }

        [DataMember(Name = "verified")]
        public bool Verified { get; set; }

        [DataMember(Name = "is_admin")]
        public bool IsAdmin { get; set; }

        [DataMember(Name = "disabled")]
        public bool Disabled { get; set; }

        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name);

        public bool ShouldSerializeAccessRole() => !string.IsNullOrEmpty(AccessRole);

        public bool ShouldSerializeEMail() => !string.IsNullOrEmpty(EMail);

        public bool ShouldSerializeHandle() => false;

        public bool ShouldSerializeVerified() => false;

        public bool ShouldSerializeIsAdmin() => false;
    }

    public class DogUserCreateParameter
    {
        [DataMember(Name = "handle")]
        public string Handle { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "access_role")]
        public string AccessRole { get; set; }

        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name);

        public bool ShouldSerializeAccessRole() => !string.IsNullOrEmpty(AccessRole);
    }

    namespace Internal
    {
        public class DogUserResult
        {
            [DataMember(Name = "user")]
            public DogUser User { get; set; }
        }

        public class DogUserGetAllResult
        {
            [DataMember(Name = "users")]
            public DogUser[] Users { get; set; }
        }
    }

    partial class DogApiClient : IUserApi
    {
        public IUserApi User => this;

        async Task<DogUser> IUserApi.CreateAsync(string handle, string name, string accessRole,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogUserCreateParameter
                {
                    Handle = handle,
                    Name = name,
                    AccessRole = accessRole
                }));
            var result = await RequestAsync<DogUserResult>(HttpMethod.Post, "/api/v1/user", null, data, cancelToken)
                .ConfigureAwait(false);
            return result.User;
        }

        async Task IUserApi.DeleteAsync(string handle, CancellationToken? cancelToken)
        {
            await RequestAsync<DogUserResult>(HttpMethod.Delete, $"/api/v1/user/{handle}", null, null, cancelToken)
                .ConfigureAwait(false);
        }

        async Task<DogUser[]> IUserApi.GetAllAsync(CancellationToken? cancelToken) =>
            (await RequestAsync<DogUserGetAllResult>(HttpMethod.Get, $"/api/v1/user", null, null, cancelToken)
                .ConfigureAwait(false)).Users;

        async Task<DogUser> IUserApi.GetAsync(string handle, CancellationToken? cancelToken) =>
            (await RequestAsync<DogUserResult>(HttpMethod.Get, $"/api/v1/user/{handle}", null, null, cancelToken)
                .ConfigureAwait(false)).User;

        async Task<DogUser> IUserApi.UpdateAsync(DogUser user, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json", JsonSerializer.Serialize(user));
            var result =
                await RequestAsync<DogUserResult>(HttpMethod.Put, $"/api/v1/user/{user.Handle}", null, data,
                    cancelToken).ConfigureAwait(false);
            return result.User;
        }
    }
}