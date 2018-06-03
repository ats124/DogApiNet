using System;
using System.Net;
using System.Runtime.Serialization;

namespace DogApiNet
{
    [Serializable]
    public class DogApiErrorException : Exception
    {
        public DogApiErrorException()
        {
        }

        public DogApiErrorException(HttpStatusCode statusCode, string[] messages)
            : base($"DogApiError StatusCode:{statusCode:D} " + string.Join(Environment.NewLine, messages))
        {
            HttpStatusCode = statusCode;
            ErrorMessages = messages;
        }

        protected DogApiErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            HttpStatusCode = (HttpStatusCode)info.GetValue(nameof(HttpStatusCode), typeof(HttpStatusCode));
            ErrorMessages = (string[])info.GetValue(nameof(ErrorMessages), typeof(string[]));
        }

        public HttpStatusCode HttpStatusCode { get; }
        public string[] ErrorMessages { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(HttpStatusCode), HttpStatusCode);
            info.AddValue(nameof(ErrorMessages), ErrorMessages);
        }
    }
}