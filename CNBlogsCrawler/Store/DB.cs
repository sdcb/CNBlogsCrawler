using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Store.Dtos;
using Neo4j.Driver.V1;
using sdmap.ext;
using Serilog;
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
        private readonly Serilog.ILogger _log;

        public DB(IDriver driver, SdmapContext sdmap, Serilog.ILogger logger)
        {
            _driver = driver;
            _sdmap = sdmap;
            _log = logger;
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

        public async Task CreateOrUpdate(User user)
        {
            using (var session = OpenSession())
            {
                await _sdmap.ExecuteAsync(session, GetId(), user);
            }
        }

        public async Task CreateIfNotExists(User user)
        {
            using (var session = OpenSession())
            {
                await _sdmap.ExecuteAsync(session, GetId(), user);
            }
        }

        public async Task CreateUserFan(string user, string fan)
        {
            using (var session = OpenSession())
            {
                await _sdmap.ExecuteAsync(session, GetId(), new
                {
                    User = user,
                    Fan = fan
                });
            }
        }

        internal async Task SetUserDone(User user)
        {
            using (var session = OpenSession())
            {
                await _sdmap.ExecuteAsync(session, GetId(), new { UserName = user.UserName });
            }
        }

        public async Task CreateUserFollow(string user, string follow)
        {
            using (var session = OpenSession())
            {
                await _sdmap.ExecuteAsync(session, GetId(), new
                {
                    User = user,
                    Follow = follow
                });
            }
        }

        internal async Task<List<User>> GetMoreUsers()
        {
            using (var session = OpenSession())
            {
                return await _sdmap.QueryListAsync<User>(session, GetId());
            }
        }

        internal async Task SaveUserFans(User user, List<User> fans)
        {
            foreach (var t in fans)
            {
                if (t.UserName.StartsWith("pick"))
                {
                    _log.Error("Invalid User: {0}", null, t.UserName);
                    continue;
                }
                await CreateIfNotExists(t);
                await CreateUserFan(user.UserName, t.UserName);
            }
        }

        internal async Task SaveUserFollows(User user, List<User> follows)
        {
            foreach (var t in follows)
            {
                if (t.UserName.StartsWith("pick"))
                {
                    _log.Error("Invalid User: {0}", null, t.UserName);
                    continue;
                }
                await CreateIfNotExists(t);
                await CreateUserFollow(user.UserName, t.UserName);
            }
        }

        public async Task<User> GetUser(string userName)
        {
            using (var session = OpenSession())
            {
                return await _sdmap.QueryFirstAsync<User>(session,
                    GetId(), new { UserName = userName });
            }
        }

        public string GetId([CallerMemberName] string callerMethod = null)
        {
            return callerMethod;
        }
    }
}
