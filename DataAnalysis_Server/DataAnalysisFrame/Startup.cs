﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataAnalysis.Component.Tools.Cache;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Web;

namespace DataAnalysisFrame
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHangfire(r => r.UseSqlServerStorage(Configuration.GetSection("Hangfire")["HangfireDB"]));
            services.BuildServiceProvider();
            return services.AddInjection();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //初始化redis
            RedisHelper.InitializeConfiguration();

            env.ConfigureNLog("Nlog.config");//读取Nlog配置文件

            //启动Hangfire服务
            app.UseHangfireServer();
            //启动hangfire面板
            app.UseHangfireDashboard();
            app.RunTask();
            //启动websocket
            WebSocketExtensions.WebSocketStart();
            app.UseMvc();
        }
    }
}
