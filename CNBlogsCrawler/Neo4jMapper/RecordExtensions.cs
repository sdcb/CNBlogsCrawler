using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogsCrawler.Neo4jMapper
{
    public static class RecordExtensions
    {
        private static T MapByRaw<T>(this IRecord record)
        {
            var t = Activator.CreateInstance<T>();
            var props = typeof(T).GetProperties();

            if (record.Keys.Count > 1)
            {
                Debugger.Break();
                var keys = new HashSet<string>(record.Keys);
                foreach (var prop in props)
                {
                    if (keys.Contains(prop.Name))
                        prop.SetValue(t, record[prop.Name]);
                }
            }
            else
            {
                var nodeProps = record[0].As<INode>().Properties;
                foreach (var prop in props)
                    if (nodeProps.ContainsKey(prop.Name))
                    {
                        prop.SetValue(t,
                            ChangeType(nodeProps[prop.Name], prop.PropertyType));
                    }
            }
            
            return t;
        }

        private static object ChangeType(object value, Type type)
        {
            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }
            return Convert.ChangeType(value, type);
        }

        public static T MapTo<T>(this IRecord record)
        {
            return MapByRaw<T>(record);
        }

        public static async Task<List<T>> MapToList<T>(this IStatementResultCursor record)
        {
            var result = new List<T>();
            while (await record.FetchAsync())
            {
                result.Add(record.Current.MapTo<T>());
            }
            return result;
        }

        public static IEnumerable<T> MapTo<T>(this IStatementResultCursor record)
        {
            while (record.FetchAsync().Result)
            {
                yield return record.Current.MapTo<T>();
            }
        }
    }
}
