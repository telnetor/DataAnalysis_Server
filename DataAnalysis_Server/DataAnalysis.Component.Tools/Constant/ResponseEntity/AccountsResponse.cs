using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant.ResponseEntity
{
    /// <summary>
    /// 用户资产实体类
    /// </summary>
    public class AccountsResponse
    {
        public long id { get; set; }
        /// <summary>
        /// 账户类型 spot：现货账户
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// working：正常, lock：账户被锁定
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int user_id { get; set; }
    }
}
