using Utf8Json;

namespace DogApiNet.JsonFormatters
{
    public class DurableBooleanFormatter : IJsonFormatter<bool>
    {
        private readonly bool _writeAsString;

        public DurableBooleanFormatter()
            : this(false)
        {
        }

        public DurableBooleanFormatter(bool writeAsString)
        {
            _writeAsString = writeAsString;
        }


        public void Serialize(ref JsonWriter writer, bool value, IJsonFormatterResolver formatterResolver)
        {
            if (_writeAsString)
                writer.WriteString(value ? "true" : "false");
            else
                writer.WriteBoolean(value);
        }

        public bool Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var token = reader.GetCurrentJsonToken();
            if (token != JsonToken.String) return reader.ReadBoolean();
            var str = reader.ReadString();
            return bool.Parse(str);

        }
    }
}