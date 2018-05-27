using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant
{

    public class ReceiveData
    {
        public string ch { get; set; }
        public long ts { get; set; }
        public Tick tick { get; set; }
    }

    public class Tick
    {
        public double[][] bids { get; set; }
        public double[][] asks { get; set; }
        public long ts { get; set; }
        public long version { get; set; }
    }

}
