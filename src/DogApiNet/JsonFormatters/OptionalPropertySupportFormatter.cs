using System;
using System.Reflection;
using Utf8Json;

namespace DogApiNet.JsonFormatters
{
    public sealed class OptionalPropertySupportFormatter<T> : IJsonFormatter<T>
        where T : OptionalPropertySupport<T>
    {
        public T Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return default(T);

            reader.ReadIsBeginObjectWithVerify();

            var obj = (T)Activator.CreateInstance(typeof(T), true);
            var propInfos = obj.OptionalPropertyInfosByJsonPropertyName;
            var backingFields = obj.BackingFields;
            var i = 0;
            var args = new object[2];
            args[1] = formatterResolver;
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref i))
            {
                var key = reader.ReadPropertyName();
                if (propInfos.TryGetValue(key, out var propInfo))
                {
                    var formatterType = typeof(IJsonFormatter<>).MakeGenericType(propInfo.PropertyType);
                    var formatter = propInfo.Formatter ?? formatterResolver.GetFormatterDynamic(propInfo.PropertyType);
                    args[0] = reader;
                    var val = formatterType.InvokeMember("Deserialize", BindingFlags.InvokeMethod, Type.DefaultBinder,
                        formatter, args);
                    backingFields[propInfo.PropertyName] = val;
                    reader = (JsonReader)args[0];
                }
                else
                {
                    var formatterType = typeof(IJsonFormatter<>).MakeGenericType(typeof(object));
                    var formatter = formatterResolver.GetFormatterDynamic(typeof(object));
                    args[0] = reader;
                    formatterType.InvokeMember("Deserialize", BindingFlags.InvokeMethod, Type.DefaultBinder,
                        formatter, args);
                    reader = (JsonReader)args[0];
                }
            }

            return obj;
        }

        public void Serialize(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteBeginObject();

                var propInfos = value.OptionalPropertyInfos;
                var args = new object[3];
                args[2] = formatterResolver;
                var first = true;
                foreach (var item in value.BackingFields)
                {
                    var propInfo = propInfos[item.Key];
                    if (propInfo.ShouldSerialize != null &&
                        ((Func<T, bool>)propInfo.ShouldSerialize)(value) == false) continue;

                    if (first)
                        first = false;
                    else
                        writer.WriteValueSeparator();

                    writer.WriteString(propInfo.JsonPropertyName ?? item.Key);
                    writer.WriteNameSeparator();
                    var formatterType = typeof(IJsonFormatter<>).MakeGenericType(propInfo.PropertyType);
                    var formatter = propInfo.Formatter ?? formatterResolver.GetFormatterDynamic(propInfo.PropertyType);
                    args[0] = writer;
                    args[1] = item.Value;
                    formatterType.InvokeMember("Serialize", BindingFlags.InvokeMethod, Type.DefaultBinder, formatter,
                        args);
                    writer = (JsonWriter)args[0];
                }

                writer.WriteEndObject();
            }
        }
    }
}