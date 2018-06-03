using System;
using Utf8Json;
using Utf8Json.Formatters;

namespace DogApiNet.JsonFormatters
{
    public class DogMetricPointFormatter : IJsonFormatter<DogMetricPoint>
    {
        public DogMetricPoint Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            reader.ReadIsBeginArrayWithVerify();

            DateTimeOffset timestamp;
            double value;
            var count = 0;
            if (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
            {
                var dec = DecimalFormatter.Default.Deserialize(ref reader,
                    formatterResolver); /* 小数点がついてくるのでDecimalFormatterを使う */
                Console.WriteLine(dec);
                timestamp = DogApiUtil.UnixTimeMillisecondsToDateTimeOffset((long)dec);
            }
            else
            {
                throw new JsonParsingException("invalid point json data");
            }

            if (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                value = reader.ReadDouble();
            else
                throw new JsonParsingException("invalid point json data");

            if (!reader.ReadIsEndArrayWithSkipValueSeparator(ref count))
                throw new JsonParsingException("invalid point json data");

            return new DogMetricPoint {Timestamp = timestamp, Value = value};
        }

        public void Serialize(ref JsonWriter writer, DogMetricPoint value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginArray();

            UnixTimeSecondsDateTimeOffsetFormatter.Default.Serialize(ref writer, value.Timestamp, formatterResolver);
            writer.WriteValueSeparator();
            writer.WriteDouble(value.Value);

            writer.WriteEndArray();
        }
    }
}