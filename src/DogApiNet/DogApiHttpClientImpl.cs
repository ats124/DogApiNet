using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DogApiNet
{
    class DogApiHttpClientImpl : DogApiHttpClient
    {
        private static readonly HttpClient httpClient = new HttpClient() { Timeout = System.Threading.Timeout.InfiniteTimeSpan }; // CancellationTokenでタイムアウトを指定するので;

        public override async Task<DogApiHttpResponseContent> RequestAsync(HttpMethod method, string url, NameValueCollection headers, NameValueCollection @params, DogApiHttpRequestContent data, TimeSpan timeOut)
        {
            using (var csc = new CancellationTokenSource(timeOut))
            {
                try
                {
                    return await RequestAsync(method, url, headers, @params, data, csc.Token);
                }
                catch (OperationCanceledException)
                {
                    throw new DogApiClientTimeoutException();
                }
            }
        }

        public override async Task<DogApiHttpResponseContent> RequestAsync(HttpMethod method, string url, NameValueCollection headers, NameValueCollection @params, DogApiHttpRequestContent requestContent, CancellationToken cancelToken)
        {
            HttpContent httpContent = null;
            try
            {
                using (var request = new HttpRequestMessage(method, CreateUrlWithQueryString(url, @params)))
                {
                    if (headers != null)
                    {
                        foreach (var headerName in headers.AllKeys)
                        {
                            request.Headers.Add(headerName, headers.GetValues(headerName));
                        }
                    }

                    if (requestContent != null)
                    {
                        httpContent = new ByteArrayContent(requestContent.Data);
                        httpContent.Headers.ContentType = new MediaTypeHeaderValue(requestContent.ContentType);
                    }

                    using (var response = await httpClient.SendAsync(request, cancelToken))
                    {
                        CheckStatusCode(response.StatusCode);
                        try
                        {
                            var content = await response.Content.ReadAsByteArrayAsync();
                            return new DogApiHttpResponseContent() { StatusCode = response.StatusCode, Data = content };
                        }
                        catch (Exception ex)
                        {
                            throw new DogApiClientHttpException(ex);
                        }
                    }
                }
            }
            finally
            {
                if (httpContent != null) httpContent.Dispose();
            }
        }

        private static string CreateUrlWithQueryString(string url, NameValueCollection @params)
        {
            if (@params == null || @params.Count == 0) return url;

            var sb = new StringBuilder(url);
            sb.Append('?');
            foreach (var paramName in @params.AllKeys)
            {
                foreach (var paramValue in @params.GetValues(paramName) ?? new[] { "" })
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] != '?') sb.Append("&");
                    sb.Append(WebUtility.UrlEncode(paramName)).Append("=").Append(WebUtility.UrlEncode(paramValue));
                }
            }
            return sb.ToString();
        }
    }
}
