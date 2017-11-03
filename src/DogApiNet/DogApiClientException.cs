using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DogApiNet
{
    [Serializable]
    public class DogApiClientException : Exception
    {
        public DogApiClientException() { }
        public DogApiClientException(string message) : base(message) { }
        public DogApiClientException(string message, Exception inner) : base(message, inner) { }
        protected DogApiClientException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class DogApiClientHttpException : DogApiClientException
    {
        public System.Net.HttpStatusCode? HttpStatusCode { get; private set; }

        public DogApiClientHttpException() { }

        public DogApiClientHttpException(System.Net.HttpStatusCode httpStatusCode) : base($"invalid http status code returned. code:{httpStatusCode:D}")
        {
            this.HttpStatusCode = httpStatusCode;
        }

        public DogApiClientHttpException(Exception inner) : base($"http error", inner) { }

        protected DogApiClientHttpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            HttpStatusCode = (System.Net.HttpStatusCode)info.GetValue(nameof(HttpStatusCode), typeof(System.Net.HttpStatusCode?));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(HttpStatusCode), HttpStatusCode);
        }
    }

    [Serializable]
    public class DogApiClientTimeoutException : DogApiClientException
    {
        public DogApiClientTimeoutException() : base("request timeout") { }

        protected DogApiClientTimeoutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class DogApiClientInvalidJsonException : DogApiClientException
    {
        public byte[] JsonData { get; private set; }

        public string JsonString => Encoding.UTF8.GetString(JsonData);

        public DogApiClientInvalidJsonException(byte[] data, Exception inner) : base("invalid json data", inner)
        {
            JsonData = data;
        }

        protected DogApiClientInvalidJsonException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            JsonData = (byte[])info.GetValue(nameof(JsonData), typeof(byte[]));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(JsonData), JsonData);
        }
    }
}
