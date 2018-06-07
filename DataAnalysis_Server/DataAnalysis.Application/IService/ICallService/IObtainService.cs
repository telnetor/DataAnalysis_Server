using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.IService.ICallService
{
    public interface IObtainService
    {
        /// <summary>
        ///  批量获取最近的交易记录
        /// </summary>
        void GetNewTransactionRecord();

        /// <summary>
        /// 获取近24小时的成交量
        /// </summary>
        MarketDetailResponse GetMmarketDetail(string bitName);

        /// <summary>
        /// 获取当前币种的行情价
        /// </summary>
        /// <returns></returns>
        //double GetMarketTrade(string bitName);

    }
}
