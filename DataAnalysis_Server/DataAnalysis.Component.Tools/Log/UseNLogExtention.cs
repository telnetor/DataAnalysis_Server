using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace DataAnalysis.Component.Tools.Log
{
    public static class UseNLogExtention
    {
        /// <summary>
        /// Use NLog for Dependency Injected loggers. 
        /// </summary>
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder)
        {
            return UseNLog(builder, null);
        }

        /// <summary>
        /// Use NLog for Dependency Injected loggers. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for logging to NLog with Dependency Injected loggers</param>
        /// <returns></returns>
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder, NLogAspNetCoreOptions options)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            options = options ?? NLogAspNetCoreOptions.Default;

            builder.ConfigureServices(services =>
            {
                //note: when registering ILoggerFactory, all non NLog stuff and stuff before this will be removed
                services.AddSingleton<ILoggerProvider>(serviceProvider =>
                {
                    return new NLogLoggerProvider(options);
                });

                //note: this one is called before  services.AddSingleton<ILoggerFactory>
                if (options.RegisterHttpContextAccessor)
                {
                    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                }

            
            });
            return builder;
        }

    }
}
