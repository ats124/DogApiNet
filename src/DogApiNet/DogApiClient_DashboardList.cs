using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;

namespace DogApiNet
{
    public interface IDashboardListApi
    {
        Task<DogDashboardList> GetAsync(long id, CancellationToken? cancelToken = null);

        Task<DogDashboardList[]> GetAllAsync(CancellationToken? cancelToken = null);

        Task<DogDashboardList> CreateAsync(string name, CancellationToken? cancelToken = null);

        Task<DogDashboardList> UpdateAsync(long id, string name, CancellationToken? cancelToken = null);

        Task DeleteAsync(long id, CancellationToken? cancelToken = null);

        Task<DogDashboard[]> GetItemsAsync(long id, CancellationToken? cancelToken = null);

        Task<DogDashboardListItem[]> AddItemsAsync(long id, DogDashboardListItem[] items,
            CancellationToken? cancelToken = null);

        Task<DogDashboardListItem[]> UpdateItemsAsync(long id, DogDashboardListItem[] items,
            CancellationToken? cancelToken = null);

        Task<DogDashboardListItem[]> DeleteItemsAsync(long id, DogDashboardListItem[] items,
            CancellationToken? cancelToken = null);
    }

    public class DogDashboardList
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "dashboard_count")]
        public int DashboardCount { get; set; }

        [DataMember(Name = "is_favorite")]
        public bool IsFavorite { get; set; }

        [DataMember(Name = "author")]
        public DogDashboardAuthor Author { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset Modified { get; set; }
    }

    public class DogDashboardListItem
    {
        public DogDashboardListItem()
        {
        }

        public DogDashboardListItem(string type, long id)
        {
            Type = type;
            Id = id;
        }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }
    }

    public class DogDashboard
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        [DataMember(Name = "is_shared")]
        public bool IsShared { get; set; }

        [DataMember(Name = "is_favorite")]
        public bool IsFavorite { get; set; }

        [DataMember(Name = "is_read_only")]
        public bool IsReadOnly { get; set; }

        [DataMember(Name = "author")]
        public DogDashboardAuthor Author { get; set; }

        [DataMember(Name = "created")]
        public DateTimeOffset Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTimeOffset Modified { get; set; }
    }

    public static class DogDashboardTypes
    {
        public static readonly string CustomTimeboard = "custom_timeboard";
        public static readonly string CustomScreenboard = "custom_screenboard";
        public static readonly string IntegrationScreenboard = "integration_screenboard";
        public static readonly string IntegrationTimeboard = "integration_timeboard";
        public static readonly string HostTimeboard = "host_timeboard";
    }

    public class DogDashboardAuthor
    {
        [DataMember(Name = "handle")]
        public string Handle { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    public class DogDashboardListCreateUpdateParameter
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    public class DogDashboardListGetAllResult
    {
        [DataMember(Name = "dashboard_lists")]
        public DogDashboardList[] DashboardLists { get; set; }
    }

    public class DogDashboardListGetItemsResult
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "dashboards")]
        public DogDashboard[] Dashboards { get; set; }
    }

    public class DogDashboardListAddUpdateItemsParameter
    {
        [DataMember(Name = "dashboards")]
        public DogDashboardListItem[] Dashboards { get; set; }
    }

    public class DogDashboardListAddItemsResult
    {
        [DataMember(Name = "added_dashboards_to_list")]
        public DogDashboardListItem[] AddedDashboardsToList { get; set; }
    }

    public class DogDashboardListUpdateItemsResult
    {
        [DataMember(Name = "dashboards")]
        public DogDashboardListItem[] Dashboards { get; set; }
    }

    public class DogDashboardListDeleteItemsResult
    {
        [DataMember(Name = "deleted_dashboards_from_list")]
        public DogDashboardListItem[] DdeletedDashboardsFromList { get; set; }
    }

    partial class DogApiClient : IDashboardListApi
    {
        public IDashboardListApi DashboardList => this;

        async Task<DogDashboardListItem[]> IDashboardListApi.AddItemsAsync(long id, DogDashboardListItem[] items,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogDashboardListAddUpdateItemsParameter {Dashboards = items}));
            var result = await RequestAsync<DogDashboardListAddItemsResult>(HttpMethod.Post,
                $"/api/v1/dashboard/lists/manual/{id}/dashboards", null,
                data, cancelToken).ConfigureAwait(false);
            return result.AddedDashboardsToList;
        }

        async Task<DogDashboardList> IDashboardListApi.CreateAsync(string name, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogDashboardListCreateUpdateParameter {Name = name}));
            return await RequestAsync<DogDashboardList>(HttpMethod.Post, $"/api/v1/dashboard/lists/manual", null,
                data, cancelToken).ConfigureAwait(false);
        }

        async Task IDashboardListApi.DeleteAsync(long id, CancellationToken? cancelToken)
        {
            await RequestAsync<DogDashboardList>(HttpMethod.Delete, $"/api/v1/dashboard/lists/manual/{id}", null,
                DogApiHttpRequestContent.EmptyJson, cancelToken).ConfigureAwait(false);
        }

        async Task<DogDashboardListItem[]> IDashboardListApi.DeleteItemsAsync(long id, DogDashboardListItem[] items,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogDashboardListAddUpdateItemsParameter {Dashboards = items}));
            var result = await RequestAsync<DogDashboardListDeleteItemsResult>(HttpMethod.Delete,
                $"/api/v1/dashboard/lists/manual/{id}/dashboards", null,
                data, cancelToken).ConfigureAwait(false);
            return result.DdeletedDashboardsFromList;
        }

        async Task<DogDashboardList[]> IDashboardListApi.GetAllAsync(CancellationToken? cancelToken)
        {
            var result = await RequestAsync<DogDashboardListGetAllResult>(HttpMethod.Get,
                $"/api/v1/dashboard/lists/manual", null,
                null, cancelToken).ConfigureAwait(false);
            return result.DashboardLists;
        }

        async Task<DogDashboardList> IDashboardListApi.GetAsync(long id, CancellationToken? cancelToken) =>
            await RequestAsync<DogDashboardList>(HttpMethod.Get, $"/api/v1/dashboard/lists/manual/{id}", null,
                null, cancelToken).ConfigureAwait(false);

        async Task<DogDashboard[]> IDashboardListApi.GetItemsAsync(long id, CancellationToken? cancelToken)
        {
            var result = await RequestAsync<DogDashboardListGetItemsResult>(HttpMethod.Get,
                $"/api/v1/dashboard/lists/manual/{id}/dashboards", null,
                null, cancelToken).ConfigureAwait(false);
            return result.Dashboards;
        }

        async Task<DogDashboardList> IDashboardListApi.UpdateAsync(long id, string name, CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogDashboardListCreateUpdateParameter {Name = name}));
            return await RequestAsync<DogDashboardList>(HttpMethod.Put, $"/api/v1/dashboard/lists/manual/{id}", null,
                data, cancelToken).ConfigureAwait(false);
        }

        async Task<DogDashboardListItem[]> IDashboardListApi.UpdateItemsAsync(long id, DogDashboardListItem[] items,
            CancellationToken? cancelToken)
        {
            var data = new DogApiHttpRequestContent("application/json",
                JsonSerializer.Serialize(new DogDashboardListAddUpdateItemsParameter {Dashboards = items}));
            var result = await RequestAsync<DogDashboardListUpdateItemsResult>(HttpMethod.Put,
                $"/api/v1/dashboard/lists/manual/{id}/dashboards", null,
                data, cancelToken).ConfigureAwait(false);
            return result.Dashboards;
        }
    }
}