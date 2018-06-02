using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utf8Json;

namespace DogApiNet.JsonFormatters
{
    public class DurableBooleanFormatter : IJsonFormatter<bool>
    {
        readonly bool writeAsString;

        public DurableBooleanFormatter()
            : this(false)
        {

        }

        public DurableBooleanFormatter(bool writeAsString)
        {
            this.writeAsString = writeAsString;
        }


        public void Serialize(ref JsonWriter writer, bool value, IJsonFormatterResolver formatterResolver)
        {
            if (writeAsString)
            {
                writer.WriteString(value ? "true" : "false");
            }
            else
            {
                writer.WriteBoolean(value);
            }
        }

        public bool Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var token = reader.GetCurrentJsonToken();
            if (token == JsonToken.String)
            {
                var str = reader.ReadString();
                return bool.Parse(str);
            }
            else
            {
                return reader.ReadBoolean();
            }
        }

    }
}