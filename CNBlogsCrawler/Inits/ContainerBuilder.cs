using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neo4j.Driver.V1;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CNBlogsCrawler.Inits
{
    public static class ContainerBuilder
    {
        public static IServiceProvider Create()
        {
            var service = new ServiceCollection();

            // logging
            service.AddSingleton<Serilog.ILogger>(new LoggerConfiguration()
                .WriteTo.RollingFile("Logs")
                .WriteTo.ColoredConsole()
                .CreateLogger());

            // configuration
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("config.json")
                .Build();
            service.AddSingleton(configuration);

            service.AddOptions();
            var neo4jSection = configuration.GetSection("Neo4j");
            service.Configure<DBSettings>(o => neo4jSection.Bind(o));

            // database
            service.AddSingleton(sp =>
            {
                var dbSetting = sp.GetService<IOptions<DBSettings>>().Value;
                return GraphDatabase.Driver(dbSetting.Url, AuthTokens.Basic(
                    dbSetting.UserName, dbSetting.Password));
            });

            // custom
            ConfigureService(service);

            return service.BuildServiceProvider();
        }

        private static void ConfigureService(IServiceCollection service)
        {
            foreach (var type in typeof(App).Assembly.GetExportedTypes()
                .Select(x => new
                {
                    Type = x, 
                    Attributes = x
                        .GetCustomAttributes(typeof(ServiceAttribute), false)
                        .OfType<ServiceAttribute>()
                        .FirstOrDefault()
                })
                .Where(x => x.Attributes != null))
            {
                service.Add(new ServiceDescriptor(
                    type.Type, type.Type, 
                    type.Attributes.Lifetime));
            }
        }
    }
}
