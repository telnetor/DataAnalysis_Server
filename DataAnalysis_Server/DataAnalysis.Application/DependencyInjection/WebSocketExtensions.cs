using DataAnalysis.Application.WebSocketExtension;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Enum;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DataAnalysisFrame
{
    public static class WebSocketExtensions
    {
        public static void WebSocketStart()
        {
            //币种
            //var dicBit = ReflectionHelper.GetStaticPropertyNameAndValue(typeof(BitContract));
            var dicBit = new Dictionary<string, object>();
            dicBit.Add("ADA_USDT", BitContract.ADA_USDT);
            CommonlyExtensions.Foreach(dicBit, TakeEnum.Depth, (topic) =>
            {
                WebSocketBehavior.Subscribe(topic, HuoBiContract.HUOBI_ID);
            });
        }
    }

}

