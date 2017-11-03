using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
namespace DogApiNet
{
    [Serializable]
    public class DogApiErrorException : Exception
    {
        public System.Net.HttpStatusCode HttpStatusCode { get; private set; }
        public string[] ErrorMessages { get; private set; }

        public DogApiErrorException() { }

        public DogApiErrorException(System.Net.HttpStatusCode statusCode, string[] messages) 
            : base($"DogApiError StatusCode:{statusCode:D} " + string.Join(Environment.NewLine, messages))
        {
            HttpStatusCode = statusCode;
            ErrorMessages = messages;
        }

        protected DogApiErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            HttpStatusCode = (System.Net.HttpStatusCode)info.GetValue(nameof(HttpStatusCode), typeof(System.Net.HttpStatusCode));
            ErrorMessages = (string[])info.GetValue(nameof(ErrorMessages), typeof(string[]));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(HttpStatusCode), HttpStatusCode);
            info.AddValue(nameof(ErrorMessages), ErrorMessages);
        }
    }
}
