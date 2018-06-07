using DataAnalysis.Manipulation.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.Entity.UserAssetEntity
{
    [Table("TAB_BLANCE")]
    public class BalanceEntity:BaseEntity
    {
        [Field(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string CurrencyName { get; set; }
        /// <summary>
        /// 交易余额
        /// </summary>
        public double TradeBalance { get; set; }

        /// <summary>
        /// 冻结余额
        /// </summary>
        public double FrozenBalance { get; set; }

    }
}
