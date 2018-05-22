using System;
using System.Collections.Generic;
using System.Text;
using Utf8Json;
using Utf8Json.Formatters;

namespace DogApiNet.JsonFormatters
{
    public sealed class StaticDictionaryFormatter<TKey, TValue, TDictionary> : IJsonFormatter<TDictionary>
        where TDictionary : class, IDictionary<TKey, TValue>, new()
    {
        readonly IJsonFormatter<TKey> keyFormatter;
        readonly IJsonFormatter<TValue> valueFormatter;

        public StaticDictionaryFormatter(Type keyFormatterType, object[] keyFormatterArguments, Type valueFormatterType, object[] valueFormatterArguments)
        {
            keyFormatter = keyFormatterType != null
                ? Activator.CreateInstance(keyFormatterType, keyFormatterArguments) as IJsonFormatter<TKey>
                : null;

            valueFormatter = valueFormatterType != null
                ? Activator.CreateInstance(valueFormatterType, keyFormatterArguments) as IJsonFormatter<TValue>
                : null;
        }

        public void Serialize(ref JsonWriter writer, TDictionary value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            else
            {
                var keyFormatter = this.keyFormatter as IObjectPropertyNameFormatter<TKey> ?? formatterResolver.GetFormatterWithVerify<TKey>() as IObjectPropertyNameFormatter<TKey>;
                if (keyFormatter == null) throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
                var valueFormatter = this.valueFormatter ?? formatterResolver.GetFormatterWithVerify<TValue>();

                writer.WriteBeginObject();

                var e = value.GetEnumerator();
                try
                {
                    if (keyFormatter != null)
                    {
                        if (e.MoveNext())
                        {
                            var item = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, item.Key, formatterResolver);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, item.Value, formatterResolver);
                        }
                        else
                        {
                            goto END;
                        }

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            var item = e.Current;
                            keyFormatter.SerializeToPropertyName(ref writer, item.Key, formatterResolver);
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, item.Value, formatterResolver);
                        }
                    }
                    else
                    {
                        if (e.MoveNext())
                        {
                            var item = e.Current;
                            writer.WriteString(item.Key.ToString());
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, item.Value, formatterResolver);
                        }
                        else
                        {
                            goto END;
                        }

                        while (e.MoveNext())
                        {
                            writer.WriteValueSeparator();
                            var item = e.Current;
                            writer.WriteString(item.Key.ToString());
                            writer.WriteNameSeparator();
                            valueFormatter.Serialize(ref writer, item.Value, formatterResolver);
                        }
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

        public TDictionary Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            else
            {
                var keyFormatter = this.keyFormatter as IObjectPropertyNameFormatter<TKey> ?? formatterResolver.GetFormatterWithVerify<TKey>() as IObjectPropertyNameFormatter<TKey>;
                if (keyFormatter == null) throw new InvalidOperationException(typeof(TKey) + " does not support dictionary key deserialize.");
                var valueFormatter = this.valueFormatter ?? formatterResolver.GetFormatterWithVerify<TValue>();

                reader.ReadIsBeginObjectWithVerify();

                var dict = new TDictionary();
                var i = 0;
                while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref i))
                {
                    var key = keyFormatter.DeserializeFromPropertyName(ref reader, formatterResolver);
                    reader.ReadIsNameSeparatorWithVerify();
                    var value = valueFormatter.Deserialize(ref reader, formatterResolver);
                    dict.Add(key, value);
                }

                return dict;
            }
        }
    }
}
