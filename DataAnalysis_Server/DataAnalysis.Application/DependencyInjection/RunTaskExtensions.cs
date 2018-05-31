
using Microsoft.AspNetCore.Builder;
using Autofac;
using Hangfire;
using DataAnalysis.Application.IService.IJobService;

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
            //RecurringJob.AddOrUpdate(() => ServerLocation._iServiceProvider.Resolve<IExecuteRedisService>().ExecuteDetpthRedisJob(), Cron.Minutely());
        }
    }
}
