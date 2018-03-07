using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Store;
using CNBlogsCrawler.Store.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogsCrawler.Crawler
{
    [Service]
    public class CrawlerSeeder
    {
        private readonly DB _db;

        public CrawlerSeeder(DB db)
        {
            _db = db;
        }

        public async Task Seed()
        {
            var user = new User
            {
                UserName = "sdflysha",
                DisplayName = "*",
                CrawlerLevel = 0,
                CrawlerStatus = CrawlerStatus.Pending,
                Avatar = $"//pic.cnblogs.com/avatar/simple_avatar.gif", 
            };
            await _db.CreateOrUpdate(user);
            var u = await _db.GetUser(user.UserName);
        }
    }
}
