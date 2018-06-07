
using Microsoft.AspNetCore.Builder;
using Autofac;
using Hangfire;
using DataAnalysis.Application.IService.IJobService;
using System;

namespace DataAnalysisFrame
{
    public static class RunTaskExtensions
    {
        public static void RunTask(this IApplicationBuilder app)
        {
            RunTask();
        }
        /// <summary>
        ///  执行任务
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void RunTask()
        {
            RecurringJob.AddOrUpdate(() => ServerLocation._iServiceProvider.Resolve<IExecuteRedisService>().ExecuteDetpthRedisJob(), Cron.Minutely());
            RecurringJob.AddOrUpdate(() => ServerLocation._iServiceProvider.Resolve<IExecuteCallService>().ExecuteBalanceJob(), Cron.Daily(0), TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate(() => ServerLocation._iServiceProvider.Resolve<IExecuteCallService>().ExecuteMarketDetailJob(), Cron.Daily(0), TimeZoneInfo.Local);
        }
    }
}
