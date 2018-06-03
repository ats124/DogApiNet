using Utf8Json;
using Utf8Json.Internal;

namespace DogApiNet.JsonFormatters
{
    public class DurableInt64Formatter : IJsonFormatter<long>
    {
        private readonly bool _writeAsNumber;

        public DurableInt64Formatter()
            : this(true)
        {
        }

        public DurableInt64Formatter(bool writeAsNumber)
        {
            _writeAsNumber = writeAsNumber;
        }

        public void Serialize(ref JsonWriter writer, long value, IJsonFormatterResolver formatterResolver)
        {
            if (_writeAsNumber)
                writer.WriteInt64(value);
            else
                writer.WriteString(value.ToString());
        }

        public long Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var token = reader.GetCurrentJsonToken();
            if (token == JsonToken.Number) return reader.ReadInt64();

            var number = reader.ReadStringSegmentRaw();
            return NumberConverter.ReadInt64(number.Array, number.Offset, out _);
        }
    }

    public class DurableNullableInt64Formatter : IJsonFormatter<long?>
    {
        private readonly bool _writeAsNumber;

        public DurableNullableInt64Formatter()
            : this(true)
        {
        }

        public DurableNullableInt64Formatter(bool writeAsNumber)
        {
            _writeAsNumber = writeAsNumber;
        }

        public void Serialize(ref JsonWriter writer, long? value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                if (_writeAsNumber)
                    writer.WriteInt64(value.Value);
                else
                    writer.WriteString(value.ToString());
            }
        }

        public long? Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;

            var token = reader.GetCurrentJsonToken();
            if (token == JsonToken.Number) return reader.ReadInt64();

            var number = reader.ReadStringSegmentRaw();
            return NumberConverter.ReadInt64(number.Array, number.Offset, out _);
        }
    }
}