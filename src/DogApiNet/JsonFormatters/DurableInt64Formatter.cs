using System;
using System.Collections.Generic;
using System.Text;
using Utf8Json;

namespace DogApiNet.JsonFormatters
{
    public class DurableInt64Formatter : IJsonFormatter<long>
    {
        bool writeAsNumber;

        public DurableInt64Formatter()
            : this(true)
        {

        }

        public DurableInt64Formatter(bool writeAsNumber)
        {
            this.writeAsNumber = writeAsNumber;
        }

        public void Serialize(ref JsonWriter writer, long value, IJsonFormatterResolver formatterResolver)
        {
            if (writeAsNumber)
            {
                writer.WriteInt64(value);
            }
            else
            {
                writer.WriteString(value.ToString());
            }
        }

        public long Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var token = reader.GetCurrentJsonToken();
            if (token == JsonToken.Number)
            {
                return reader.ReadInt64();
            }
            else
            {
                var number = reader.ReadStringSegmentRaw();
                return Utf8Json.Internal.NumberConverter.ReadInt64(number.Array, number.Offset, out _);
            }
        }
    }
}
