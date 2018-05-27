
using DataAnalysis.Application.Service.BitService;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.Service
{
    public class BitFactory
    {
        public static BitBaseService GetSingle(string chType)
        {
            BitBaseService bitBase = null;
            var array = chType.Split('.');
            if (array.Length == 4)
            {
                //币种名称
                string bitName = array[1].Replace("usdt", "");
                //深度或K线
                string type = array[2];
                switch (bitName)
                {
                    case "adausdt":
                        bitBase = new AdaService();
                        break;
                    default:
                        bitBase = new BitBaseService();
                        break;
                }
            }
            return bitBase;
        }
    }
}
