using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Utf8Json;
using Utf8Json.Formatters;

namespace DogApiNet.JsonFormatters
{
    public class WeekDaysFormatter : IJsonFormatter<DayOfWeek[]>
    {
        private static readonly IDictionary<string, DayOfWeek> NameValueMappings;
        private static readonly IDictionary<DayOfWeek, string> ValueNameMappings;
        private readonly ArrayFormatter<string> _internalFormatter = new ArrayFormatter<string>();

        static WeekDaysFormatter()
        {
            ValueNameMappings = new Dictionary<DayOfWeek, string>
            {
                {DayOfWeek.Sunday, "Sun"},
                {DayOfWeek.Monday, "Mon"},
                {DayOfWeek.Tuesday, "Tue"},
                {DayOfWeek.Wednesday, "Wed"},
                {DayOfWeek.Thursday, "Thu"},
                {DayOfWeek.Friday, "Fri"},
                {DayOfWeek.Saturday, "Sat"},
            };
            NameValueMappings = ValueNameMappings.ToDictionary(x => x.Value, x => x.Key);
        }

        public DayOfWeek[] Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) =>
            _internalFormatter.Deserialize(ref reader, formatterResolver)?.Select(x => NameValueMappings[x]).ToArray();

        public void Serialize(ref JsonWriter writer, DayOfWeek[] value, IJsonFormatterResolver formatterResolver) =>
            _internalFormatter.Serialize(ref writer, value?.Select(x => ValueNameMappings[x]).ToArray(),
                formatterResolver);
    }
}