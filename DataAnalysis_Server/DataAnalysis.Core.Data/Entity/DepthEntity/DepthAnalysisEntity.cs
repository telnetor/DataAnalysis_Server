using DataAnalysis.Manipulation.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.Entity.DepthEntity
{
    [Table("TAB_DEPTH_ANALYSIS")]
    public class DepthAnalysisEntity : BaseEntity
    {
        [Field(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string CurrencyName { get; set; }


        public double ForecastAmount { get; set; }

        public long ServerReturnTime { get; set; }

        public string ForecastTime { get; set; }

    }
}
