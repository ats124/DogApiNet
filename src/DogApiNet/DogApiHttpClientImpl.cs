using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DogApiNet
{
    internal class DogApiHttpClientImpl : DogApiHttpClient
    {
        private static readonly HttpClient HttpClient;

        static DogApiHttpClientImpl()
        {
            var handler = new HttpClientHandler {UseCookies = false};
            HttpClient = new HttpClient(handler) {Timeout = Timeout.InfiniteTimeSpan};
        }

        public DogApiHttpClientImpl(string dataDogHost)
        {
            ServicePointManager.FindServicePoint(new Uri(dataDogHost)).ConnectionLeaseTimeout = 60 * 1000;
        }

        public override async Task<DogApiHttpResponseContent> RequestAsync(HttpMethod method, string url,
            NameValueCollection headers, NameValueCollection @params, DogApiHttpRequestContent data, TimeSpan timeOut)
        {
            using (var csc = new CancellationTokenSource(timeOut))
            {
                try
                {
                    return await RequestAsync(method, url, headers, @params, data, csc.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw new DogApiClientTimeoutException();
                }
            }
        }

        public override async Task<DogApiHttpResponseContent> RequestAsync(HttpMethod method, string url,
            NameValueCollection headers, NameValueCollection @params, DogApiHttpRequestContent requestContent,
            CancellationToken cancelToken)
        {
            using (var request = new HttpRequestMessage(method, CreateUrlWithQueryString(url, @params)))
            {
                if (headers != null)
                    foreach (var headerName in headers.AllKeys)
                        request.Headers.Add(headerName, headers.GetValues(headerName));

                if (requestContent != null)
                {
                    var httpContent = new ByteArrayContent(requestContent.Data);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue(requestContent.ContentType);
                    request.Content = httpContent;
                }

                using (var response = await HttpClient.SendAsync(request, cancelToken).ConfigureAwait(false))
                {
                    try
                    {
                        var responseHeaders = response.Headers;
                        var limit = GetResponseHeaderValue(responseHeaders, "X-RateLimit-Limit");
                        var period = GetResponseHeaderValue(responseHeaders, "X-RateLimit-Period");
                        var remaining = GetResponseHeaderValue(responseHeaders, "X-RateLimit-Remaining");
                        var reset = GetResponseHeaderValue(responseHeaders, "X-RateLimit-Reset");
                        DogApiRateLimit rateLimit = null;
                        if (limit != null && period != null && remaining != null && reset != null)
                            rateLimit = new DogApiRateLimit(limit.Value, period.Value, remaining.Value, reset.Value);

                        var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                        return new DogApiHttpResponseContent
                        {
                            StatusCode = response.StatusCode,
                            Data = content,
                            MediaType = response.Content.Headers?.ContentType?.MediaType,
                            RateLimit = rateLimit
                        };
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new DogApiClientHttpException(ex);
                    }
                }
            }
        }

        private static string CreateUrlWithQueryString(string url, NameValueCollection @params)
        {
            if (@params == null || @params.Count == 0) return url;

            var sb = new StringBuilder(url);
            sb.Append('?');
            foreach (var paramName in @params.AllKeys)
            foreach (var paramValue in @params.GetValues(paramName) ?? new[] {""})
            {
                if (sb.Length > 0 && sb[sb.Length - 1] != '?') sb.Append("&");
                sb.Append(WebUtility.UrlEncode(paramName)).Append("=").Append(WebUtility.UrlEncode(paramValue));
            }

            return sb.ToString();
        }

        private int? GetResponseHeaderValue(HttpResponseHeaders header, string name)
        {
            if (header.TryGetValues(name, out var values))
            {
                var value = values.FirstOrDefault();
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var intValue)) return intValue;
            }

            return null;
        }
    }
}