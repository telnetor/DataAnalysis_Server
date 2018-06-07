using DataAnalysis.Manipulation.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.Entity.CallEntity
{
    [Table("TAB_MARKET_DETAIL")]
    public class MarketDetailEntity:BaseEntity
    {
        [Field(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 币名
        /// </summary>
        public string CurrencyName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 24小时成交量
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 前推24小时成交价
        /// </summary>
        public double Open { get; set; }
        /// <summary>
        /// 当前成交价
        /// </summary>
        public double Close { get; set; }
        /// <summary>
        /// 近24小时最高价
        /// </summary>
        public double High { get; set; }
        /// <summary>
        ///  24小时统计时间
        /// </summary>
        public long Ts { get; set; }
        /// <summary>
        /// 消息id
        /// </summary>
        public long MarketId { get; set; }
        /// <summary>
        /// 近24小时累积成交数
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 近24小时最低价
        /// </summary>
        public double Low { get; set; }
        /// <summary>
        /// 近24小时累积成交额, 即 sum(每一笔成交价 * 该笔的成交量)
        /// </summary>
        public double Vol { get; set; }
    }
}
