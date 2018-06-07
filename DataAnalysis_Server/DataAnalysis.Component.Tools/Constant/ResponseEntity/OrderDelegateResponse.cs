using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant.ResponseEntity
{

    public class OrderDelegateResponse
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 交易对
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// 账户 ID
        /// </summary>
        [JsonProperty(PropertyName = "account-id")]
        public int accountid { get; set; }
        /// <summary>
        /// 订单数量
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// 订单价格
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 订单撤销时间
        /// </summary>
        [JsonProperty(PropertyName = "created-at")]
        public long createdat { get; set; }
        /// <summary>
        /// 订单类型  
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 已成交数量
        /// </summary>
        [JsonProperty(PropertyName = "field-amount")]
        public string fieldamount { get; set; }
        /// <summary>
        /// 已成交总金额
        /// </summary>
        [JsonProperty(PropertyName = "field-cash-amount")]
        public string fieldcashamount { get; set; }
        /// <summary>
        /// 已成交手续费（买入为币，卖出为钱）
        /// </summary>
        [JsonProperty(PropertyName = "field-fees")]
        public string fieldfees { get; set; }
        /// <summary>
        /// 最后成交时间
        /// </summary>
        [JsonProperty(PropertyName = "finished-at")]
        public long finishedat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userid { get; set; }
        /// <summary>
        /// 订单来源
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 订单撤销时间
        /// </summary>
        [JsonProperty(PropertyName = "canceled-at")]
        public int canceledat { get; set; }
        /// <summary>
        /// 来自
        /// </summary>
        public string exchange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string batch { get; set; }
    }

}
