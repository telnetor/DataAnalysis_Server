using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.IService.IJobService
{
    public interface IExecuteCallService
    {
        /// <summary>
        /// 每天0点准时获取金额
        /// </summary>
        void ExecuteBalanceJob();

        /// <summary>
        /// 每天0点获取24小时成交量数据
        /// </summary>
        void ExecuteMarketDetailJob();
    }
}
