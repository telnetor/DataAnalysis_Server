using DataAnalysis.Manipulation.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.TempEntity
{
    public class DepthTempBuyEntity
    {
        /// <summary>
        /// 币名称
        /// </summary>
        public string CrrencyName { get; set; }
        /// <summary>
        /// 币类型 深度 or KLine
        /// </summary>
        public string CrrencyType { get; set; }
        /// <summary>
        /// 最大成交量
        /// </summary>
        public float MaxVolume { get; set; }
        /// <summary>
        /// 最大成交量对应价格
        /// </summary>
        public float MaxPrice { get; set; }
        /// <summary>
        /// 最小成交量
        /// </summary>
        public float MinVolume { get; set; }
        /// <summary>
        /// 最小成交量对应价格
        /// </summary>
        public float MinPrice { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long ts { get; set; }

        /// <summary>
        /// 总成交量
        /// </summary>
        public float TotalVolume { get; set; }

        /// <summary>
        /// 总成交价格
        /// </summary>
        public float TotalPrice { get; set; }

    }
}

