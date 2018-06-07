using DataAnalysis.Application.IService.ICallService;
using DataAnalysis.Core.Data.Entity.DepthEntity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DataAnalysis.Component.Tools.Cache;

namespace DataAnalysis.Application.Service.CallService
{
    public class StrategyService : IStrategyService
    {

        public StrategyService()
        {
        }

        public void LargeStrategy(List<BitDetailEntity> bidsList, List<BitDetailEntity> asksList)
        {
            //分别取出买入单的大单和卖出单的大单
            var tempBids = bidsList.Where(p => p.Number == bidsList.Max(o => o.Number)).Select((t, i) => new
            { i, t.Number, t.Price }).FirstOrDefault();
            var tempAsks = asksList.Where(p => p.Number == asksList.Max(o => o.Number)).Select((t, i) => new
            { i, t.Number, t.Price }).FirstOrDefault();
            


        }
    }
}
