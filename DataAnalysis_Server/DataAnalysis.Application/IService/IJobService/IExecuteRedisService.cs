
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.IService.IJobService
{
    public interface IExecuteRedisService
    {
        /// <summary>
        /// 定时执行深度队列
        /// </summary>
        void ExecuteDetpthRedisJob();
    }
}
