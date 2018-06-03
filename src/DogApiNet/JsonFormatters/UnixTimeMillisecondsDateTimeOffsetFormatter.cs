using System;
using Utf8Json;

namespace DogApiNet.JsonFormatters
{
    public sealed class UnixTimeMillisecondsDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset>
    {
        public static readonly IJsonFormatter<DateTimeOffset> Default =
            new UnixTimeMillisecondsDateTimeOffsetFormatter();

        public DateTimeOffset Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver) =>
            DogApiUtil.UnixTimeMillisecondsToDateTimeOffset(reader.ReadInt64());

        public void Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteInt64(value.ToUnixTimeMilliseconds());
        }
    }

    public sealed class NullableUnixTimeMillisecondsDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset?>
    {
        public static readonly IJsonFormatter<DateTimeOffset?> Default =
            new NullableUnixTimeMillisecondsDateTimeOffsetFormatter();

        public DateTimeOffset? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
                return null;
            return DogApiUtil.UnixTimeMillisecondsToDateTimeOffset(reader.ReadInt64());
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset? value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteInt64(value.Value.ToUnixTimeMilliseconds());
        }
    }
}