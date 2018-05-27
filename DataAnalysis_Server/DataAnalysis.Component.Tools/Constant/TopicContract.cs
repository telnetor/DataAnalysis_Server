using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant
{
    public class TopicContract
    {
        //{ ethbtc, ltcbtc, etcbtc, bchbtc...... }
        /*
         * $period 可选值：{ 1min, 5min, 15min, 30min, 60min, 1day, 1mon, 1week, 1year }
         */
        public static string K_LINE = "market.$symbol.kline.$period";

        /*
         * $type 可选值：{ step0, step1, step2, step3, step4, ep5 } （合并深度0-5）；step0时，不合并深度
         */
        public static string MARKET_DEPTH = "market.$symbol.depth.$type";

        public static string TRADE_DETAIL = "market.$symbol.trade.detail";

        public static string MARKET_DETAIL = "market.$symbol.detail";
    }
}
