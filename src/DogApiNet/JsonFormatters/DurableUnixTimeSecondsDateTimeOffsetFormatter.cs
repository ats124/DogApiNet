using System;
using Utf8Json;
using Utf8Json.Internal;

namespace DogApiNet.JsonFormatters
{
    public sealed class DurableUnixTimeSecondsDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset>
    {
        public static readonly IJsonFormatter<DateTimeOffset> Default =
            new DurableUnixTimeSecondsDateTimeOffsetFormatter();

        private readonly bool _writeAsNumber;

        public DurableUnixTimeSecondsDateTimeOffsetFormatter() : this(true)
        {
        }

        public DurableUnixTimeSecondsDateTimeOffsetFormatter(bool writeAsNumber)
        {
            _writeAsNumber = writeAsNumber;
        }

        public DateTimeOffset Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var token = reader.GetCurrentJsonToken();
            long lngValue;
            if (token == JsonToken.Number)
            {
                lngValue = reader.ReadInt64();
            }
            else
            {
                var number = reader.ReadStringSegmentRaw();
                lngValue = NumberConverter.ReadInt64(number.Array, number.Offset, out _);
            }

            return DogApiUtil.UnixTimeSecondsToDateTimeOffset(lngValue);
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
        {
            var seconds = value.ToUnixTimeSeconds();
            if (_writeAsNumber)
                writer.WriteInt64(seconds);
            else
                writer.WriteString(seconds.ToString());
        }
    }

    public sealed class DurableNullableUnixTimeSecondsDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset?>
    {
        public static readonly IJsonFormatter<DateTimeOffset?> Default =
            new DurableNullableUnixTimeSecondsDateTimeOffsetFormatter();

        private readonly bool _writeAsNumber;

        public DurableNullableUnixTimeSecondsDateTimeOffsetFormatter() : this(true)
        {
        }

        public DurableNullableUnixTimeSecondsDateTimeOffsetFormatter(bool writeAsNumber)
        {
            _writeAsNumber = writeAsNumber;
        }

        public DateTimeOffset? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;

            var token = reader.GetCurrentJsonToken();
            if (token == JsonToken.Number)
            {
                reader.ReadInt64();
            }
            else
            {
                var number = reader.ReadStringSegmentRaw();
                NumberConverter.ReadInt64(number.Array, number.Offset, out var readCount);
                if (readCount == 0) return null;
            }

            return DogApiUtil.UnixTimeSecondsToDateTimeOffset(reader.ReadInt64());
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset? value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var seconds = value.Value.ToUnixTimeSeconds();
                if (_writeAsNumber)
                    writer.WriteInt64(seconds);
                else
                    writer.WriteString(seconds.ToString());
            }
        }
    }
}