using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Utf8Json;
using Utf8Json.Formatters;

namespace DogApiNet.JsonFormatters
{
    public sealed class OptionalPropertySupportFormatter<T> : IJsonFormatter<T> 
        where T : OptionalPropertySupport<T>, new()
    {
        public OptionalPropertySupportFormatter()
        {
        }

        public T Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return default(T);
            }
            else
            {
                reader.ReadIsBeginObjectWithVerify();

                var obj = new T();
                var propInfos = obj.OptionalPropertyInfosByJsonPropertyName;
                var backingFields = obj.BackingFields;
                var i = 0;
                var args = new object[2];
                args[1] = formatterResolver;
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref i))
                {
                    var key = reader.ReadPropertyName();
                    var propInfo = propInfos[key];
                    var formatterType = typeof(IJsonFormatter<>).MakeGenericType(propInfo.PropertyType);
                    var formatter = propInfo.Formatter ?? formatterResolver.GetFormatterDynamic(propInfo.PropertyType);
                    args[0] = reader;
                    var val = formatterType.InvokeMember("Deserialize", BindingFlags.InvokeMethod, Type.DefaultBinder, formatter, args);
                    backingFields[propInfo.PropertyName] = val;
                    reader = (JsonReader)args[0];
                }

                return obj;
            }
        }

        public void Serialize(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            else
            {
                writer.WriteBeginObject();

                var propInfos = value.OptionalPropertyInfos;
                var e = value.BackingFields.GetEnumerator();
                var args = new object[3];
                args[2] = formatterResolver;
                try
                {
                    if (e.MoveNext())
                    {
                        var item = e.Current;
                        var propInfo = propInfos[item.Key];
                        writer.WriteString(propInfo.JsonPropertyName ?? item.Key);
                        writer.WriteNameSeparator();
                        var formatterType = typeof(IJsonFormatter<>).MakeGenericType(propInfo.PropertyType);
                        var formatter = propInfo.Formatter ?? formatterResolver.GetFormatterDynamic(propInfo.PropertyType);
                        args[0] = writer;
                        args[1] = item.Value;
                        formatterType.InvokeMember("Serialize", BindingFlags.InvokeMethod, Type.DefaultBinder, formatter, args);
                        writer = (JsonWriter)args[0];
                    }
                    else
                    {
                        goto END;
                    }

                    while (e.MoveNext())
                    {
                        writer.WriteValueSeparator();
                        var item = e.Current;
                        var propInfo = propInfos[item.Key];
                        writer.WriteString(propInfo.JsonPropertyName ?? item.Key);
                        writer.WriteNameSeparator();
                        var formatterType = typeof(IJsonFormatter<>).MakeGenericType(propInfo.PropertyType);
                        var formatter = propInfo.Formatter ?? formatterResolver.GetFormatterDynamic(propInfo.PropertyType);
                        args[0] = writer;
                        args[1] = item.Value;
                        formatterType.InvokeMember("Serialize", BindingFlags.InvokeMethod, Type.DefaultBinder, formatter, args);
                        writer = (JsonWriter)args[0];
                    }
                }
                finally
                {
                    e.Dispose();
                }

                END:
                writer.WriteEndObject();
            }
        }
    }
}
