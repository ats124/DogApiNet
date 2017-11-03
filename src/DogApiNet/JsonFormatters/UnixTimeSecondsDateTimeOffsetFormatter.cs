using System;
using System.Collections.Generic;
using System.Text;

using Utf8Json;

namespace DogApiNet.JsonFormatters
{
    public sealed class UnixTimeSecondsDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset>
    {
        public static readonly IJsonFormatter<DateTimeOffset> Default = new UnixTimeSecondsDateTimeOffsetFormatter();

        public DateTimeOffset Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            return DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt64());
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteInt64(value.ToUnixTimeSeconds());
        }
    }

    public sealed class NullableUnixTimeSecondsDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset?>
    {
        public static readonly IJsonFormatter<DateTimeOffset?> Default = new NullableUnixTimeSecondsDateTimeOffsetFormatter();

        public DateTimeOffset? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            else
            {
                return DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt64());
            }
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset? value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteInt64(value.Value.ToUnixTimeSeconds());
            }
        }
    }
}
