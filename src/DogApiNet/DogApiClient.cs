using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

using Utf8Json;
using System.Text;

namespace DogApiNet
{
    public partial class DogApiClient : IDisposable
    {
        public static readonly string DefaultDataDogHost = "https://app.datadoghq.com";

        public string ApiKey { get; private set; }

        public string AppKey { get; private set; }

        public string DataDogHost { get; private set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);

        private bool leaveDispose;

        private DogApiHttpClient httpClient;

        public DogApiClient(string apiKey, string appKey = null, string dataDogHost = null, DogApiHttpClient httpClient = null, bool leaveDispose = true)
        {
            this.ApiKey = apiKey;
            this.AppKey = appKey;
            this.DataDogHost = dataDogHost ?? DefaultDataDogHost;
            this.httpClient = httpClient ?? new DogApiHttpClientImpl();
            this.leaveDispose = leaveDispose;
        }

        private async Task<T> RequestAsync<T>(HttpMethod method, string path, NameValueCollection @params, DogApiHttpRequestContent data, CancellationToken? cancelToken)
        {
            @params = @params == null 
                ? new NameValueCollection()
                : new NameValueCollection(@params);
            @params.Add("api_key", ApiKey);
            if (AppKey != null)
            {
                @params.Add("application_key", AppKey);
            }
            var url = DataDogHost + path;

            DogApiHttpResponseContent content;
            try
            {
                content = await (cancelToken.HasValue
                    ? httpClient.RequestAsync(method, url, null, @params, data, cancelToken.Value).ConfigureAwait(false)
                    : httpClient.RequestAsync(method, url, null, @params, data, Timeout).ConfigureAwait(false));
            }
            catch (DogApiClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DogApiClientException("http request error", ex);
            }


            if ((int)content.StatusCode >= 200 && (int)content.StatusCode < 300)
            {
                T result;
                try
                {
                    result = JsonSerializer.Deserialize<T>(content.Data);
                }
                catch (Exception ex)
                {
                    throw new DogApiClientInvalidJsonException(content.Data, ex);
                }

                return result;
            }
            else
            {
                if (content.MediaType == "application/json")
                {
                    DogApiErrorInfo errorInfo;
                    try
                    {
                        errorInfo = JsonSerializer.Deserialize<DogApiErrorInfo>(content.Data);
                    }
                    catch (Exception ex)
                    {
                        throw new DogApiClientInvalidJsonException(content.Data, ex);
                    }
                    throw new DogApiErrorException(content.StatusCode, errorInfo.Errors);
                }
                else
                {
                    throw new DogApiClientHttpException(content.StatusCode);
                }
            }
        }

        [ThreadStatic]
        private static StringBuilder _buffer;
        public static (string key, string value) DeconstructTag(string tag)
        {
            if (_buffer == null || _buffer.Capacity > 256)
            {
                _buffer = new StringBuilder(256);
            }
            else
            {
                _buffer.Clear();
            }

            int i = 0;
            for (; i < tag.Length; i++)
            {
                if (tag[i] == ':')
                {
                    i++;
                    break;
                }
                else
                {
                    _buffer.Append(tag[i]);
                }
            }
            var key = _buffer.ToString();

            _buffer.Clear();
            for (; i < tag.Length; i++)
            {
                _buffer.Append(tag[i]);
            }
            var value = _buffer.ToString();

            return (key, value);
        }
        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (httpClient != null && leaveDispose)
                    {
                        httpClient.Dispose();
                        httpClient = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    public class DogApiErrorInfo
    {
        [DataMember(Name = "errors")]
        public string[] Errors { get; set; }
    }
}
