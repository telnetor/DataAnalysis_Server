using DataAnalysis.Manipulation.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.Entity.TradeEntity
{
    [Table("TAB_TRACE")]
    public class TraceDataEntity : BaseEntity
    {
        [Field(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        /// 币名
        /// </summary>
        public string CurrencyName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string TransactionDate { get; set; }
    }
}
