using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.IService.IJobService
{
    /// <summary>
    /// 定时执行队列接口 
    /// </summary>
    public interface IExecuteQueueService
    {
        /// <summary>
        /// 定时执行深度队列
        /// </summary>
        void ExecuteDetpthQueueJob();
        /// <summary>
        /// 定时执行K线队列
        /// </summary>
        void ExecuteKLineQueueJob();
    }
}
