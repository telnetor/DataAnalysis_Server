using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant
{
    public class HuoBiContract
    {
        /// <summary>
        /// GUID
        /// </summary>
        public static string HUOBI_ID = Guid.NewGuid().ToString().Replace("-", string.Empty);

        /// <summary>
        /// topic
        /// </summary>
        public static Dictionary<string, string> topicDic = new Dictionary<string, string>();
    }
}
