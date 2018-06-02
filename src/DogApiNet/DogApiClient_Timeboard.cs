using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Formatters;

namespace DogApiNet
{
    using JsonFormatters;

    public interface ITimeboardApi
    {
    }

    partial class DogApiClient : ITimeboardApi
    {
        public ITimeboardApi Timeboard => this;
    }
}
