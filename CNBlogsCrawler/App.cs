using CNBlogsCrawler.Crawler;
using CNBlogsCrawler.Inits;
using CNBlogsCrawler.Store;
using CNBlogsCrawler.Store.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neo4j.Driver.V1;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace CNBlogsCrawler
{
    public static class App
    {
        static async Task Main(string[] args)
        {            
            var container = ContainerBuilder.Create();
            var db = container.GetService<DB>();
            await db.EnsureConstraint();

            var seeder = container.GetService<CrawlerSeeder>();
            await seeder.Seed();
        }
    }
}
