using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Store.Dtos;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogsCrawler.Crawler
{
    [Service]
    public class CrawlerServer
    {
        private readonly ILogger _logger;

        public CrawlerServer(ILogger logger)
        {
            _logger = logger;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                List<User> users = await GetMoreUsers();
                _logger.Information($"More users: {users.Count}");
                if (users.Count == 0) break;

                foreach (var user in users)
                {
                    _logger.Information($"CrawlUser: {user.UserName}/{user.DisplayName}");
                    await CrawlUser(user);
                }
            }
        }

        private Task CrawlUser(User user)
        {
            throw new NotImplementedException();
        }

        private Task<List<User>> GetMoreUsers()
        {
            throw new NotImplementedException();
        }
    }
}
