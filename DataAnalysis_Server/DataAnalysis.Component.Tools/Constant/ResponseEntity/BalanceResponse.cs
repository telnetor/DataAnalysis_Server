using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant.ResponseEntity
{
    public class BalanceResponse
    {
        public int id { get; set; }
        public string type { get; set; }
        public string state { get; set; }
        public List<Balance> list { get; set; }
    }
    public class Balance
    {
        public string currency { get; set; }
        public string type { get; set; }
        public string balance { get; set; }
    }
}
