using Autofac;
using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Log;
using DataAnalysisFrame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;



namespace DataAnalysis.Application.WebSocketExtension
{
    public class HuoBiMessageReceivedEventArgs : EventArgs
    {

        private ReceiveData receiveData { get; set; }

        public HuoBiMessageReceivedEventArgs(ReceiveData _receiveData)
        {
            this.receiveData = _receiveData;
            InsertData();
        }
        private void InsertData()
        {
            try
            {
                string ch = receiveData.ch;
                if (ch.IndexOf("depth") > 0)
                {
                    var service = ServerLocation._iServiceProvider.Resolve<IExecuteSocketService>();
                    service.ExecuteDetpthJob(receiveData);
                }
                else if (ch.IndexOf("kline") > 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                    });
                }
            }
            catch (Exception ex)
            {
                LogManage.Error.Debug(ex);
            }

        }
    }
}
