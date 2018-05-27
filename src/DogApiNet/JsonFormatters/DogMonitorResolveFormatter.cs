using System;
using System.Collections.Generic;
using System.Text;
using Utf8Json;
using Utf8Json.Formatters;

namespace DogApiNet.JsonFormatters
{
    public class DogMonitorResolveFormatter : IJsonFormatter<DogMonitorResolve>
    {
        public DogMonitorResolve Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            reader.ReadIsBeginObjectWithVerify();

            var stringFormatter = formatterResolver.GetFormatterWithVerify<string>();
            var keyFormatter = (IObjectPropertyNameFormatter<string>)stringFormatter;

            long monitorId = 0;
            string group = "";
            var i = 0;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref i))
            {
                monitorId = long.Parse(keyFormatter.DeserializeFromPropertyName(ref reader, formatterResolver));
                reader.ReadIsNameSeparatorWithVerify();
                group = stringFormatter.Deserialize(ref reader, formatterResolver);
            }

            return new DogMonitorResolve(monitorId, group);
        }

        public void Serialize(ref JsonWriter writer, DogMonitorResolve value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteBeginObject();

            var stringFormatter = formatterResolver.GetFormatterWithVerify<string>();
            var keyFormatter = (IObjectPropertyNameFormatter<string>)stringFormatter;
            keyFormatter.SerializeToPropertyName(ref writer, value.MonitorId.ToString(), formatterResolver);
            writer.WriteNameSeparator();
            stringFormatter.Serialize(ref writer, value.Group, formatterResolver);

            writer.WriteEndObject();
        }
    }
}
