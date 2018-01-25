using CNBlogsCrawler.Inits;
using Neo4j.Driver.V1;
using sdmap.Compiler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogsCrawler.Sqls
{
    [Service]
    public class SdmapNeo4j
    {
        private readonly SdmapCompiler _compiler;

        public SdmapNeo4j(SdmapCompiler compiler)
        {
            _compiler = compiler;
        }

        public Task<IStatementResultCursor> RunAsync(ISession session, 
            string statementId, object parameters)
        {
            var statement = _compiler.Emit(statementId, parameters);
            return session.RunAsync(statement, parameters);
        }

        internal Task RunAsync(ISession session, 
            string statementId)
        {
            var statement = _compiler.Emit(statementId, null);
            return session.RunAsync(statement);
        }
    }
}
