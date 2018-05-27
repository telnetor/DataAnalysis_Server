using DataAnalysis.Manipulation.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.TempEntity
{
    public class DepthTempEntity
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
        /// 单笔成交量
        /// </summary>
        public double SingleVolume { get; set; }
        /// <summary>
        /// 单笔成交价格
        /// </summary>
        public double SinglePrice { get; set; }

        /// <summary>
        /// 单笔成交总价
        /// </summary>
        public double SingleTotal { get; set; }

    }
}
