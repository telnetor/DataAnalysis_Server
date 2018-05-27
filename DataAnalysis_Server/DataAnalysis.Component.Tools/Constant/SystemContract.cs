using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Constant
{
    public class SystemContract
    {

        /// <summary>
        /// 深度安全队列
        /// </summary>

        public static ConcurrentQueue<ReceiveData> messageDepthQueue = new ConcurrentQueue<ReceiveData>();

        /// <summary>
        /// K线安全队列
        /// </summary>
        public static ConcurrentQueue<ReceiveData> messageKLineQueue = new ConcurrentQueue<ReceiveData>();
    }
}
