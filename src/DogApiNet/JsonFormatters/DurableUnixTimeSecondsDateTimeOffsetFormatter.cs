using System;
using System.Collections.Generic;
using System.Text;

using Utf8Json;

namespace DogApiNet.JsonFormatters
{
    public sealed class DurableUnixTimeSecondsDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset>
    {
        public static readonly IJsonFormatter<DateTimeOffset> Default = new DurableUnixTimeSecondsDateTimeOffsetFormatter();

        bool writeAsNumber;

        public DurableUnixTimeSecondsDateTimeOffsetFormatter() : this(true)
        {
        }

        public DurableUnixTimeSecondsDateTimeOffsetFormatter(bool writeAsNumber)
        {
            this.writeAsNumber = writeAsNumber;
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
                lngValue = Utf8Json.Internal.NumberConverter.ReadInt64(number.Array, number.Offset, out _);
            }

            return DogApiUtil.UnixTimeSecondsToDateTimeOffset(lngValue);
        }

        public void Serialize(ref JsonWriter writer, DateTimeOffset value, IJsonFormatterResolver formatterResolver)
        {
            var seconds = value.ToUnixTimeSeconds();
            if (writeAsNumber)
            {
                writer.WriteInt64(seconds);
            }
            else
            {
                writer.WriteString(seconds.ToString());
            }
        }
    }

    public sealed class DurableNullableUnixTimeSecondsDateTimeOffsetFormatter : IJsonFormatter<DateTimeOffset?>
    {
        public static readonly IJsonFormatter<DateTimeOffset?> Default = new DurableNullableUnixTimeSecondsDateTimeOffsetFormatter();

        bool writeAsNumber;

        public DurableNullableUnixTimeSecondsDateTimeOffsetFormatter() : this(true)
        {
        }

        public DurableNullableUnixTimeSecondsDateTimeOffsetFormatter(bool writeAsNumber)
        {
            this.writeAsNumber = writeAsNumber;
        }

        public DateTimeOffset? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            else
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
                    lngValue = Utf8Json.Internal.NumberConverter.ReadInt64(number.Array, number.Offset, out var readCount);
                    if (readCount == 0)
                    {
                        return null;
                    }
                }
                return DogApiUtil.UnixTimeSecondsToDateTimeOffset(reader.ReadInt64());
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
                var seconds = value.Value.ToUnixTimeSeconds();
                if (writeAsNumber)
                {
                    writer.WriteInt64(seconds);
                }
                else
                {
                    writer.WriteString(seconds.ToString());
                }
            }
        }
    }
}
