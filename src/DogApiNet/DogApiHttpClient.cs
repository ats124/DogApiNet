using System;
using System.Linq;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace DogApiNet
{
    public abstract class DogApiHttpClient : IDisposable
    {
        public abstract Task<DogApiHttpResponseContent> RequestAsync(HttpMethod method, string url, NameValueCollection headers, NameValueCollection @params, DogApiHttpRequestContent data, TimeSpan timeOut);

        public abstract Task<DogApiHttpResponseContent> RequestAsync(HttpMethod method, string url, NameValueCollection headers, NameValueCollection @params, DogApiHttpRequestContent data, CancellationToken cancelToken);

        protected static readonly HttpStatusCode[] EXISTS_DATA_ERROR_STATUS_CODE = new[]
        {
            HttpStatusCode.BadRequest, HttpStatusCode.Forbidden, HttpStatusCode.Conflict, HttpStatusCode.Gone
        };

        protected void CheckStatusCode(HttpStatusCode code)
        {
            if (((int)code < 200 || (int)code >= 300) && Array.IndexOf(EXISTS_DATA_ERROR_STATUS_CODE, code) < 0)
            {
                throw new DogApiClientHttpException(code);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
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
        public byte[] Data { get; set; }
    }

    public class DogApiHttpRequestContent
    {
        public string ContentType { get; }
        public byte[] Data { get; }

        public DogApiHttpRequestContent(string contentType, byte[] content)
        {
            if (contentType == null) throw new ArgumentNullException(nameof(contentType));
            if (content == null) throw new ArgumentNullException(nameof(content));
            ContentType = contentType;
            Data = content;
        }
    }
}
