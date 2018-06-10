using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DogApiNet
{
    public abstract class DogApiHttpClient : IDisposable
    {
        public abstract Task<DogApiHttpResponseContent> RequestAsync(HttpMethod method, string url,
            NameValueCollection headers, NameValueCollection @params, DogApiHttpRequestContent data, TimeSpan timeOut);

        public abstract Task<DogApiHttpResponseContent> RequestAsync(HttpMethod method, string url,
            NameValueCollection headers, NameValueCollection @params, DogApiHttpRequestContent data,
            CancellationToken cancelToken);

        #region IDisposable Support

        private bool _disposedValue; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
            }

            _disposedValue = true;
        }

        ~DogApiHttpClient()
        {
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    public class DogApiHttpResponseContent
    {
        public HttpStatusCode StatusCode { get; set; }
        public string MediaType { get; set; }
        public byte[] Data { get; set; }
        public DogApiRateLimit RateLimit {get; set; }
    }

    public class DogApiHttpRequestContent
    {
        public DogApiHttpRequestContent(string contentType, byte[] content)
        {
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            Data = content ?? throw new ArgumentNullException(nameof(content));
        }

        public static DogApiHttpRequestContent EmptyJson { get; } =
            new DogApiHttpRequestContent("application/json", new byte[0]);

        public string ContentType { get; }
        public byte[] Data { get; }
    }
}