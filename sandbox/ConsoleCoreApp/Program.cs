using System;
using System.Runtime.Serialization;
using Utf8Json;
using DogApiNet;
using DogApiNet.Json;
using DogApiNet.JsonFormatters;

namespace ConsoleCoreApp
{
    [JsonFormatter(typeof(OptionalPropertySupportFormatter<TestClass>))]
    public class TestClass : OptionalPropertySupport<TestClass>
    {
        public string TestPropA { get => (string)GetValue(); set => SetValue(value); }

        public long TestPropB { get => (int)GetValue(); set => SetValue(value); }

        [DataMember(Name = "propc")]
        [JsonFormatter(typeof(Utf8Json.Formatters.UnixTimestampDateTimeFormatter))]
        public DateTime TestPropC { get => (DateTime)GetValue(); set => SetValue(value); }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var obj = new TestClass();
            Console.WriteLine(JsonSerializer.ToJsonString(obj));    // output: {}

            obj.TestPropA = "hoge";
            Console.WriteLine(JsonSerializer.ToJsonString(obj));    // output: {"TestPropA":"hoge"}

            obj.TestPropB = 1000;
            Console.WriteLine(JsonSerializer.ToJsonString(obj));    // output: {"TestPropA":"hoge","TestPropB":1000}

            obj.TestPropC = new DateTime(1970, 1, 1, 0, 0, 10, DateTimeKind.Utc);
            Console.WriteLine(JsonSerializer.ToJsonString(obj));    // output: {"TestPropA":"hoge","TestPropB":100,"TestPropC":10}

            obj.TestPropA = null;
            Console.WriteLine(JsonSerializer.ToJsonString(obj));    // output: {"TestPropA":null,"TestPropB":100,"TestPropC":10}

            obj.Unset(x => x.TestPropA);
            Console.WriteLine(JsonSerializer.ToJsonString(obj));    // output: {"TestPropB":100,"TestPropC":10}
        }
    }
}
