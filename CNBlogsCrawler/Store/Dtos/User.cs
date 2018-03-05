using System;
using System.Collections.Generic;
using System.Text;

namespace CNBlogsCrawler.Store.Dtos
{
    public class User
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Avatar { get; set; }

        public int CrawlerLevel { get; set; }

        public CrawlerStatus CrawlerStatus { get; set; }
    }

    public enum CrawlerStatus
    {
        Pending, 
        Done, 
    }
}
