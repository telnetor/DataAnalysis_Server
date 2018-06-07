using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysisFrame
{
    public static class CommonlyExtensions
    {
        public static void Foreach(this Dictionary<string, object> dic, TakeEnum takeEnum, Action<string> action)
        {
            //深度(step5)
            var depthStep = DepthContract.STEP_ZERO;
            //K线 (15分钟)
            var kLineMinute = KLineContract.FIFTEEN_MINUTE;

            //读取 topic格式
            string kLine = TopicContract.K_LINE;
            //深度
            string marketDepth = TopicContract.MARKET_DEPTH;
            //成交价格
            string tradeDetail = TopicContract.TRADE_DETAIL;

            foreach (KeyValuePair<string, object> item in dic)
            {
                string key = item.Key;
                string value = Convert.ToString(item.Value);
                string topic = string.Empty;
                if (takeEnum == TakeEnum.ALL)
                {
                    //深度
                    var depthTopic = marketDepth.Replace("$symbol", value).Replace("$type", depthStep);
                    //读取K线
                    //var kLineTopic = kLine.Replace("$symbol", value).Replace("$period", kLineMinute);
                    //读取最新成交价格
                    var tradeTopic = tradeDetail.Replace("$symbol", value);
                    action(depthTopic);
                    //action(kLineTopic);
                    action(tradeTopic);
                    continue;
                }
                else if (takeEnum == TakeEnum.Depth)
                {
                    //深度
                    topic = marketDepth.Replace("$symbol", value).Replace("$type", depthStep);
                }
                else if (takeEnum == TakeEnum.KLine)
                {
                    //读取K线 market.$symbol.kline.$period
                    topic = kLine.Replace("$symbol", value).Replace("$period", kLineMinute);
                }
                else if (takeEnum == TakeEnum.Trade)
                {
                    //成交价格
                    topic = tradeDetail.Replace("$symbol", value);
                }
                action(topic);
            }
        }
    }
}
