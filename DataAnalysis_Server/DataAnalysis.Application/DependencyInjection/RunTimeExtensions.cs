using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Log;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DataAnalysis.Application.DependencyInjection
{
    public static class RunTimeExtensions
    {
        private static TimeSpan _checkBrokenLine = TimeSpan.FromMilliseconds(1000*10);
        private static Timer _checkBrokenTimer;
        private static  object _checkBrokenLineLock = new object();
        private static volatile bool _checkBrokenLineFlg = false;

        //深度队列
        private static volatile int lastDepthQueueCount = 0;
        private static volatile int failCount = 0;

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
            _checkBrokenTimer = new Timer(CheckBrokenLine, null, _checkBrokenLine, _checkBrokenLine);
        }
        /// <summary>
        /// 检测是否短线
        /// </summary>
        private static  void CheckBrokenLine(object state)
        {
            lock (_checkBrokenLineLock)
            {
                try
                {
                    if (!_checkBrokenLineFlg)
                    {
                        _checkBrokenLineFlg = true;
                    }
                }
                catch (Exception ex)
                {
                    LogManage.Job.Debug(ex);
                }
                finally
                {
                    _checkBrokenLineFlg = false;
                }

            }
        }
    }
}
