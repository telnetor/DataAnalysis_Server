using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DataAnalysis.Manipulation.WebSocketExtension
{
    public class HuoBiMessageReceivedEventArgs : EventArgs
    {
       
        private ReceiveData receiveData { get; set; }

        public HuoBiMessageReceivedEventArgs(ReceiveData _receiveData)
        {
            this.receiveData = _receiveData;
            InsertDataToQueue();
        }
        private void InsertDataToQueue()
        {
            try
            {
                string ch = receiveData.ch;
                if (ch.IndexOf("depth") > 0)
                {
                    SystemContract.messageDepthQueue.Enqueue(receiveData);
                    //if (SystemContract.messageDepthQueue.Count % 10 == 0)
                    //{
                    //    Trace.WriteLine("深度" + SystemContract.messageDepthQueue.Count);
                    //}

                }
                else if (ch.IndexOf("kline") > 0)
                {
                    SystemContract.messageKLineQueue.Enqueue(receiveData);
                    //Trace.WriteLine("KLine" + SystemContract.messageDepthQueue.Count);
                }
            }
            catch (Exception ex)
            {
                LogManage.Error.Debug(ex);
            }
           
        }
    }
}
