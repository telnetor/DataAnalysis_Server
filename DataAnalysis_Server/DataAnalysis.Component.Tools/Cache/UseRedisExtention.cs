using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace DataAnalysis.Component.Tools.Cache
{
    public static class UseRedisExtention
    {
        /// <summary>
        /// Use NLog for Dependency Injected loggers. 
        /// </summary>
        public static IWebHostBuilder UseRedis(this IWebHostBuilder builder)
        {
            return UseRedis(builder, null);
        }

        /// <summary>
        /// Use NLog for Dependency Injected loggers. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for logging to NLog with Dependency Injected loggers</param>
        /// <returns></returns>
        public static IWebHostBuilder UseRedis(this IWebHostBuilder builder, NLogAspNetCoreOptions options)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            options = options ?? NLogAspNetCoreOptions.Default;

            builder.ConfigureServices(services =>
            {
               
                services.AddSingleton<ICacheManager>(serviceProvider =>
                {
                    return new RedisCacheManager();
                });


            
            });
            return builder;
        }

    }
}
