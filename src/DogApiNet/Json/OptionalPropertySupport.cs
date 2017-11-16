using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Utf8Json;

namespace DogApiNet.Json
{
    public abstract class OptionalPropertySupport<T> where T : OptionalPropertySupport<T>
    {
        static readonly IReadOnlyDictionary<string, OptionalPropertyInfo> optionalPropertyInfos;
        static readonly IReadOnlyDictionary<string, OptionalPropertyInfo> optionalPropertyInfosByJsonPropertyName;

        internal IReadOnlyDictionary<string, OptionalPropertyInfo> OptionalPropertyInfos => optionalPropertyInfos;

        internal IReadOnlyDictionary<string, OptionalPropertyInfo> OptionalPropertyInfosByJsonPropertyName => optionalPropertyInfosByJsonPropertyName;

        internal readonly Dictionary<string, object> BackingFields;

        static OptionalPropertySupport()
        {
            var infos = new Dictionary<string, OptionalPropertyInfo>();
            foreach (var prop in typeof(T).GetProperties())
            {
                string jsonFieldName = null;
                object formatter = null;
                foreach (var attr in prop.GetCustomAttributes(false))
                {
                    if (attr is DataMemberAttribute dataMember)
                    {
                        jsonFieldName = dataMember.Name;
                    }
                    else if (attr is JsonFormatterAttribute json)
                    {
                        formatter = Activator.CreateInstance(json.FormatterType, json.Arguments) ;
                    }
                }
                infos[prop.Name] = new OptionalPropertyInfo(prop.Name, prop.PropertyType, jsonFieldName, formatter);
            }

            optionalPropertyInfos = new ReadOnlyDictionary<string, OptionalPropertyInfo>(infos);
            optionalPropertyInfosByJsonPropertyName = new ReadOnlyDictionary<string, OptionalPropertyInfo>(infos.Values.ToDictionary(x => x.JsonPropertyName ?? x.PropertyName));
        }

        public OptionalPropertySupport()
        {
            BackingFields = new Dictionary<string, object>();
        }

        protected void SetValue(object value, [CallerMemberName] string member = "")
            => BackingFields[member] = value;

        protected object GetValue([CallerMemberName] string member = "")
            => BackingFields[member];

        protected bool IsSet(string member)
            => BackingFields.ContainsKey(member);

        protected bool Unset(string member)
            => BackingFields.Remove(member);

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
        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }
        
        public string JsonPropertyName { get; private set; }

        public object Formatter { get; private set; }

        public OptionalPropertyInfo(string propertyName, Type propertyType, string jsonFieldName, object formatter)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            JsonPropertyName = jsonFieldName;
            Formatter = formatter;
        }
    }
}
