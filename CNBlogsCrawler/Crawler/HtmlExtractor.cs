using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Store.Dtos;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNBlogsCrawler.Crawler
{
    [Service]
    public class HtmlExtractor
    {
        public List<User> ExtractUsers(string html, int crawlerLevel)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes(@"//*[@id='main']/div[2]/ul/li");
            if (nodes == null) return new List<User>();

            var result = new List<User>();
            foreach (HtmlNode node in nodes)
            {
                User user = ExtractUserFromNode(node);
                user.CrawlerLevel = crawlerLevel;
                result.Add(user);
            }
            return result;
        }

        private static User ExtractUserFromNode(HtmlNode node)
        {
            return new User
            {
                Avatar = node.SelectSingleNode(@"a[1]/div[1]/img").GetAttributeValue("src", null),
                CrawlerStatus = CrawlerStatus.Pending,
                DisplayName = node.SelectSingleNode(@"a[1]").GetAttributeValue("title", null),
                UserName = node.SelectSingleNode(@"a[1]").GetAttributeValue("href", null).Replace("/u/", "")
            };
        }
    }
}
