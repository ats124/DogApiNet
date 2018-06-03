using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace DogApiNet
{
    [Serializable]
    public class DogApiClientException : Exception
    {
        public DogApiClientException()
        {
        }

        public DogApiClientException(string message) : base(message)
        {
        }

        public DogApiClientException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DogApiClientException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class DogApiClientHttpException : DogApiClientException
    {
        public DogApiClientHttpException()
        {
        }

        public DogApiClientHttpException(HttpStatusCode httpStatusCode) : base(
            $"invalid http status code returned. code:{httpStatusCode:D}")
        {
            HttpStatusCode = httpStatusCode;
        }

        public DogApiClientHttpException(Exception inner) : base($"http error", inner)
        {
        }

        protected DogApiClientHttpException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            HttpStatusCode = (HttpStatusCode)info.GetValue(nameof(HttpStatusCode), typeof(HttpStatusCode?));
        }

        public HttpStatusCode? HttpStatusCode { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(HttpStatusCode), HttpStatusCode);
        }
    }

    [Serializable]
    public class DogApiClientTimeoutException : DogApiClientException
    {
        public DogApiClientTimeoutException() : base("request timeout")
        {
        }

        protected DogApiClientTimeoutException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class DogApiClientInvalidJsonException : DogApiClientException
    {
        public DogApiClientInvalidJsonException(byte[] data, Exception inner) : base("invalid json data", inner)
        {
            JsonData = data;
        }

        protected DogApiClientInvalidJsonException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            JsonData = (byte[])info.GetValue(nameof(JsonData), typeof(byte[]));
        }

        public byte[] JsonData { get; }

        public string JsonString => Encoding.UTF8.GetString(JsonData);

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(JsonData), JsonData);
        }
    }
}