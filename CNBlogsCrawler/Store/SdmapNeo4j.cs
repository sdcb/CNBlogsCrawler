using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Store.Internal;
using Neo4j.Driver.V1;
using sdmap.Compiler;
using System;
using System.Collections.Generic;
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
    }
}
