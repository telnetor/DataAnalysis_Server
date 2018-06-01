using DataAnalysis.Manipulation.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Core.Data.Entity.DepthEntity
{
    [Table("TAB_DEPTH_ANALYSIS")]
    public class DepthAnalysisEntity
    {
        [Field(IsPrimaryKey = true, IsIdentity = true, ColumnName = "ID")]
        public int Id { get; set; }

        [Field(ColumnName = "CURRENCY_NAME")]
        public string CurrencyName { get; set; }


        [Field(ColumnName = "FORECAST_AMOUNT")]
        public double ForecastAmount { get; set; }

        [Field(ColumnName = "SERVER_RETURN_TIME")]
        public long ServerReturnTime { get; set; }

        [Field(ColumnName = "FORECAST_TIME")]
        public string ForecastTime { get; set; }

    }
}
