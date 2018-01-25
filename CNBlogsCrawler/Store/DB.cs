using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Sqls;
using CNBlogsCrawler.Store.Dtos;
using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogsCrawler.Store
{
    [Service]
    public class DB
    {
        private readonly IDriver _driver;
        private readonly SdmapNeo4j _sdmap;

        public DB(IDriver driver, SdmapNeo4j sdmap)
        {
            _driver = driver;
            _sdmap = sdmap;
        }

        private ISession OpenSession()
            => _driver.Session();

        public async Task EnsureConstraint()
        {
            using (var session = OpenSession())
            {
                await _sdmap.RunAsync(session, GetId());
            }
        }

        public async Task CreateUser(User user)
        {
            using (var session = OpenSession())
            {
                await _sdmap.RunAsync(session, GetId(), new { props = user });
            }
        }

        public string GetId([CallerMemberName] string callerMethod = null)
        {
            return callerMethod;
        }
    }
}
