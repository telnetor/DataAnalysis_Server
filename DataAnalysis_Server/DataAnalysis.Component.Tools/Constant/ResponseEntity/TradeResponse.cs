using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant.ResponseEntity
{
    /// <summary>
    /// 批量获取最近的交易记录
    /// </summary>
    public class TradeResponse 
    {
        public string status { get; set; }
        public string ch { get; set; }
        public long ts { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public long id { get; set; }
        public long ts { get; set; }
        public Datum1[] data { get; set; }
    }

    public class Datum1
    {
        [JsonProperty(PropertyName = "layerId")]
        public long id { get; set; }
        public double amount { get; set; }
        public long price { get; set; }
        public string direction { get; set; }
        public long ts { get; set; }
    }
}
