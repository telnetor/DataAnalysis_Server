using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.IService.IJobService
{
    /// <summary>
    /// 定时执行队列接口 
    /// </summary>
    public interface IExecuteSocketService
    {
        /// <summary>
        /// 执行深度
        /// </summary>
        void ExecuteDetpthJob(ReceiveDataSocket receiveData);
        /// <summary>
        /// 执行K线
        /// </summary>
        void ExecuteKLineJob();

        /// <summary>
        /// 更新redis中币种的最新价格
        /// </summary>
        void ExecuteTradeJob(TraceDataSocket receiveData);
    }
}
