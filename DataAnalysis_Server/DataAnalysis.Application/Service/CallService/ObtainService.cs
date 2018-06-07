using DataAnalysis.Application.IService.ICallService;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using DataAnalysis.Component.Tools.Enum;
using DataAnalysis.Component.Tools.Http;
using DataAnalysis.Component.Tools.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DataAnalysis.Application.Service.CallService
{
    public class ObtainService: IObtainService
    {
        /// <summary>
        /// 获取近24小时的成交量
        /// </summary>
        public MarketDetailResponse GetMmarketDetail(string bitName)
        {
            // /market/detail?symbol=ethusdt
            string resourcePath = AppSetting.GetConnection("HuoBi", "GetMmarketDetail");
            string json=HttpRestHelper.SendRequest(resourcePath, $"symbol={bitName}usdt");
            var res = JsonConvert.DeserializeObject<MarketDetailResponse>(json);
            if (res.status.Equals(StatusEnum.ok.ToString()))
            {
                return res;
            }
            return null;
        }

        /// <summary>
        /// 获取当前币种的行情价
        /// </summary>
        /// <returns></returns>
        //public double GetMarketTrade(string bitName)
        //{
        //    //https://api.huobi.br.com/market/trade?symbol=adausdt
        //    string resourcePath = AppSetting.GetConnection("HuoBi", "GetMarketTrade");
        //    HttpRestHelper.SendRequest(resourcePath, $"symbol={bitName}usdt");
        //}

        /// <summary>
        ///  批量获取最近的交易记录
        /// </summary>
        public void GetNewTransactionRecord()
        {
            ///market/history/trade
            string resourcePath = AppSetting.GetConnection("HuoBi", "GetNewTransactionRecord");
            string json = HttpRestHelper.SendRequest(resourcePath, "symbol=adausdt");
           
            var res=JsonConvert.DeserializeObject<TradeResponse>(json);
        }
    }
}
