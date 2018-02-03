using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Store.Dtos;
using Neo4j.Driver.V1;
using sdmap.ext;
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
        private readonly SdmapContext _sdmap;

        public DB(IDriver driver, SdmapContext sdmap)
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
                await _sdmap.ExecuteAsync(session, GetId());
            }
        }

        public async Task CreateUser(User user)
        {
            using (var session = OpenSession())
            {
                await _sdmap.ExecuteAsync(session, GetId(), new { props = user });
            }
        }

        public async Task<bool> UserExists(User user)
        {
            using (var session = OpenSession())
            {
                return await _sdmap.ExecuteScalarAsync<bool>(session, 
                    GetId(), new { UserName = user.UserName});
            }
        }

        public string GetId([CallerMemberName] string callerMethod = null)
        {
            return callerMethod;
        }
    }
}
