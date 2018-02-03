using Neo4j.Driver.V1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CNBlogsCrawler.Store.Internal
{
    internal static class CollectionExtensions
    {
        public static IDictionary<string, object> ToDictionary(this object o)
        {
            if (o == null)
            {
                return null;
            }

            return FillDictionary(o, new Dictionary<string, object>());
        }

        private static IDictionary<string, object> FillDictionary(object o, IDictionary<string, object> dict)
        {
            foreach (var propInfo in o.GetType().GetRuntimeProperties())
            {
                var name = propInfo.Name;
                var value = propInfo.GetValue(o);
                var valueTransformed = Transform(value);

                dict.Add(name, valueTransformed);
            }

            return dict;
        }

        private static object Transform(object value)
        {
            if (value == null)
            {
                return null;
            }

            var valueType = value.GetType();

            if (value is Array)
            {
                var elementType = valueType.GetElementType();

                if (elementType.NeedsConversion())
                {
                    var convertedList = new List<object>(((IList)value).Count);
                    foreach (var element in (IEnumerable)value)
                    {
                        convertedList.Add(Transform(element));
                    }
                    value = convertedList;
                }
            }
            else if (value is IList)
            {
                var valueTypeInfo = valueType.GetTypeInfo();
                var elementType = (Type)null;

                if (valueTypeInfo.IsGenericType && valueTypeInfo.GetGenericTypeDefinition() == typeof(List<>))
                {
                    elementType = valueTypeInfo.GenericTypeArguments[0];
                }

                if (elementType == null || elementType.NeedsConversion())
                {
                    var convertedList = new List<object>(((IList)value).Count);
                    foreach (var element in (IEnumerable)value)
                    {
                        convertedList.Add(Transform(element));
                    }
                    value = convertedList;
                }
            }
            else if (value is IDictionary)
            {
                var valueTypeInfo = valueType.GetTypeInfo();
                var elementType = (Type)null;

                if (valueTypeInfo.IsGenericType && valueTypeInfo.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    elementType = valueTypeInfo.GenericTypeArguments[1];
                }

                if (elementType == null || elementType.NeedsConversion())
                {
                    var dict = (IDictionary)value;

                    var convertedDict = new Dictionary<string, object>(dict.Count);
                    foreach (object key in dict.Keys)
                    {
                        if (!(key is string))
                        {
                            throw new InvalidOperationException(
                                "dictionaries passed as part of a parameter to cypher statements should have string keys!");
                        }

                        convertedDict.Add((string)key, Transform(dict[key]));
                    }
                    value = convertedDict;
                }
            }
            else if (value is Enum)
            {
                return Convert.ToInt64(value);
            }
            else
            {
                if (valueType.NeedsConversion())
                {
                    value = FillDictionary(value, new Dictionary<string, object>());
                }
            }

            return value;
        }

        private static bool NeedsConversion(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }

            if (type.GetTypeInfo().IsValueType)
            {
                return false;
            }

            return true;
        }
    }
}
