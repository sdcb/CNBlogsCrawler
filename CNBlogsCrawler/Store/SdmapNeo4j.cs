using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Neo4jMapper;
using CNBlogsCrawler.Store.Internal;
using Neo4j.Driver.V1;
using sdmap.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading.Tasks;

namespace sdmap.ext
{
    public static class SdmapExtensions
    {
        public static async Task ExecuteAsync(
            this SdmapContext ctx, 
            ISession session, 
            string statementId, 
            object parameters = null)
        {
            var statement = ctx.Emit(statementId, parameters);
            var cursor = await session.RunAsync(statement, parameters.ToDictionary());
        }

        public static async Task<List<T>> QueryListAsync<T>(
            this SdmapContext ctx, 
            ISession session, 
            string statementId, 
            object parameters = null)
        {
            var statement = ctx.Emit(statementId, parameters);
            var cursor = await session.RunAsync(statement, parameters.ToDictionary());
            var result = new List<T>();
            while (await cursor.FetchAsync())
            {
                IRecord record = cursor.Current;
                result.Add(record.MapTo<T>());
            }
            return result;
        }

        public static async Task<T> ExecuteScalarAsync<T>(
            this SdmapContext ctx, 
            ISession session,
            string statementId, 
            object parameters = null)
        {
            var statement = ctx.Emit(statementId, parameters);
            var cursor = await session.RunAsync(statement, parameters.ToDictionary());
            var fected = await cursor.FetchAsync();
            return fected ?
                cursor.Current[0].As<T>() : default;
        }

        public static async Task<T> QueryFirstAsync<T>(
            this SdmapContext ctx,
            ISession session,
            string statementId,
            object parameters = null)
        {
            var statement = ctx.Emit(statementId, parameters);
            var cursor = await session.RunAsync(statement, parameters.ToDictionary());
            if (!await cursor.FetchAsync()) throw new Exception();
            return cursor.Current.MapTo<T>();
        }
    }
}
