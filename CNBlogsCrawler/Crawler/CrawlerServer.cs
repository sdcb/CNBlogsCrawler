using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Store;
using CNBlogsCrawler.Store.Dtos;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CNBlogsCrawler.Crawler
{
    [Service]
    public class CrawlerServer
    {
        private readonly ILogger _logger;
        private readonly DB _db;
        private readonly HttpClient _http;
        private readonly HtmlExtractor _extractor;

        public CrawlerServer(
            ILogger logger, 
            DB db, 
            HttpClient http, 
            HtmlExtractor extractor)
        {
            _logger = logger;
            _db = db;
            _http = http;
            _extractor = extractor;
        }

        public async Task StartAsync()
        {
            await Login();
            while (true)
            {
                List<User> users = await _db.GetMoreUsers();
                _logger.Information($"More users: {users.Count}");
                if (users.Count == 0) break;

                foreach (var user in users)
                {
                    _logger.Information($"CrawlUser: {user.UserName}/{user.DisplayName}");
                    await CrawlUser(user);
                    await _db.SetUserDone(user);
                }
            }
        }

        private Task Login()
        {
            _http.DefaultRequestHeaders.Add("Cookie", null);
            return Task.FromResult(0);
        }

        private async Task CrawlUser(User user)
        {
            List<User> follows = await GetUserFollows(user);
            await _db.SaveUserFollows(user, follows);
            
            List<User> fans = await GetUserFans(user);
            await _db.SaveUserFans(user, fans);
        }

        private async Task<List<User>> GetUserFans(User user)
        {
            var result = new List<User>();
            for (var page = 1; ; page++)
            {
                string url = $"https://home.cnblogs.com/u/{user.UserName}/relation/followers?page={page}";
                string html = await _http.GetStringAsync(url);
                List<User> users = _extractor.ExtractUsers(html, user.CrawlerLevel + 1);
                if (users.Count == 0) break;
                result.AddRange(users);
            }
            return result;
        }

        private async Task<List<User>> GetUserFollows(User user)
        {
            var result = new List<User>();
            for (var page = 1; ; page++)
            {
                string url = $"https://home.cnblogs.com/u/{user.UserName}/relation/following?page={page}";
                string html = await _http.GetStringAsync(url);
                List<User> users = _extractor.ExtractUsers(html, user.CrawlerLevel + 1);
                if (users.Count == 0) break;
                result.AddRange(users);                
            }
            return result;
        }
    }
}
