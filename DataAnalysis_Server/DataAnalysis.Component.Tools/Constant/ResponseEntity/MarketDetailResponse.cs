using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant.ResponseEntity
{

    public class MarketDetailResponse
    {
        public string status { get; set; }
        public string ch { get; set; }
        public long ts { get; set; }
        public Tick tick { get; set; }
    }

    public class Tick
    {
        public double amount { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        [JsonProperty(PropertyName = "layerTs")]
        public long ts { get; set; }
        public long id { get; set; }
        public long count { get; set; }
        public double low { get; set; }
        public double vol { get; set; }
    }
}
