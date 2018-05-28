using DataAnalysis.Manipulation.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.BitEntity
{
    [Table("tb_Depth")]
    public class DepthEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// 币名称
        /// </summary>
        public string CrrencyName { get; set; }
        /// <summary>
        /// 币类型 深度 or KLine
        /// </summary>
        public string CrrencyType { get; set; }

        /// <summary>
        /// 买入总成交量
        /// </summary>
        public float TotalBuyVolume { get; set; }

        /// <summary>
        /// 买入总成交价格
        /// </summary>
        public float TotalBuyPrice { get; set; }

        /// <summary>
        /// 卖出总成交量
        /// </summary>
        public float TotalSellingVolume { get; set; }

        /// <summary>
        /// 卖出总成交价格
        /// </summary>
        public float TotalSellingPrice { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long ts { get; set; }



    }
}

