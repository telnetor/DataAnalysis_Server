using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant.ResponseEntity
{
    public class TraceDataSocket
    {
        public string ch { get; set; }
        [JsonProperty(PropertyName = "ts")]
        public long traceTs { get; set; }
        public Tick tick { get; set; }

        public class Tick
        {
            [JsonProperty(PropertyName = "id")]
            public long tickId { get; set; }
            [JsonProperty(PropertyName = "ts")]
            public long tickTs { get; set; }
            public Datum[] data { get; set; }
        }

        public class Datum
        {
            public double amount { get; set; }
            [JsonProperty(PropertyName = "ts")]
            public long datumTs { get; set; }
            [JsonProperty(PropertyName = "id")]
            public double datumId { get; set; }
            public double price { get; set; }
            public string direction { get; set; }
        }
    }

}
