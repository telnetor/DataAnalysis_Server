using DataAnalysis.Core.Data.Entity.DepthEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.IService.ICallService
{
    public interface IStrategyService
    {
        /// <summary>
        /// 大单策略
        /// </summary>
        void LargeStrategy(List<BitDetailEntity> bidsList, List<BitDetailEntity> asksList);
    }
}
