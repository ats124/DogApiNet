using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Utf8Json;

namespace DogApiNet.JsonFormatters
{
    public abstract class OptionalPropertySupport<T> where T : OptionalPropertySupport<T>
    {
        private static readonly IReadOnlyDictionary<string, OptionalPropertyInfo> optionalPropertyInfos;

        private static readonly IReadOnlyDictionary<string, OptionalPropertyInfo>
            optionalPropertyInfosByJsonPropertyName;

        internal readonly Dictionary<string, object> BackingFields;

        static OptionalPropertySupport()
        {
            var infos = new Dictionary<string, OptionalPropertyInfo>();
            foreach (var prop in typeof(T).GetProperties())
            {
                string jsonFieldName = null;
                object formatter = null;
                foreach (var attr in prop.GetCustomAttributes(false))
                    switch (attr)
                    {
                        case DataMemberAttribute dataMember:
                            jsonFieldName = dataMember.Name;
                            break;
                        case JsonFormatterAttribute json:
                            formatter = Activator.CreateInstance(json.FormatterType, json.Arguments);
                            break;
                    }

                var shouldSerialize = typeof(T).GetMethod($"ShouldSerialize{prop.Name}");
                var shouldSerializeDelegate =
                    shouldSerialize != null ? Delegate.CreateDelegate(typeof(Func<T, bool>), shouldSerialize) : null;
                infos[prop.Name] = new OptionalPropertyInfo(prop.Name, prop.PropertyType, jsonFieldName, formatter,
                    shouldSerializeDelegate);
            }

            optionalPropertyInfos = new ReadOnlyDictionary<string, OptionalPropertyInfo>(infos);
            optionalPropertyInfosByJsonPropertyName =
                new ReadOnlyDictionary<string, OptionalPropertyInfo>(
                    infos.Values.ToDictionary(x => x.JsonPropertyName ?? x.PropertyName));
        }

        protected OptionalPropertySupport()
        {
            BackingFields = new Dictionary<string, object>();
        }

        internal IReadOnlyDictionary<string, OptionalPropertyInfo> OptionalPropertyInfos => optionalPropertyInfos;

        internal IReadOnlyDictionary<string, OptionalPropertyInfo> OptionalPropertyInfosByJsonPropertyName =>
            optionalPropertyInfosByJsonPropertyName;

        protected void SetValue<T1>(T1 value, [CallerMemberName] string member = "")
        {
            BackingFields[member] = value;
        }

        protected T1 GetValue<T1>(T1 defaultValue = default(T1), [CallerMemberName] string member = "")
        {
            return BackingFields.TryGetValue(member, out var value) ? (T1)value : defaultValue;
        }

        protected bool IsSet(string member)
        {
            return BackingFields.ContainsKey(member);
        }

        protected bool Unset(string member)
        {
            return BackingFields.Remove(member);
        }

        public bool IsSet<TValue>(Expression<Func<T, TValue>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            if (memberExpression == null) throw new InvalidOperationException("Expression must be a member expression");

            return IsSet(memberExpression.Member.Name);
        }

        public bool Unset<TValue>(Expression<Func<T, TValue>> member)
        {
            var memberExpression = member.Body as MemberExpression;
            if (memberExpression == null) throw new InvalidOperationException("Expression must be a member expression");

            return Unset(memberExpression.Member.Name);
        }
    }

    internal class OptionalPropertyInfo
    {
        public OptionalPropertyInfo(string propertyName, Type propertyType, string jsonFieldName, object formatter,
            Delegate shouldSerialize)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            JsonPropertyName = jsonFieldName;
            Formatter = formatter;
            ShouldSerialize = shouldSerialize;
        }

        public string PropertyName { get; }

        public Type PropertyType { get; }

        public string JsonPropertyName { get; }

        public object Formatter { get; }

        public Delegate ShouldSerialize { get; }
    }
}