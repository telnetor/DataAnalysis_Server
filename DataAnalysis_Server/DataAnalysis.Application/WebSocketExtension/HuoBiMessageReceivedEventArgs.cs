using Autofac;
using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using DataAnalysis.Component.Tools.Log;
using DataAnalysisFrame;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;



namespace DataAnalysis.Application.WebSocketExtension
{
    public class HuoBiMessageReceivedEventArgs : EventArgs
    {
        private string _msg { get; set; }
        

        public HuoBiMessageReceivedEventArgs(string msg)
        {
            this._msg = msg;

            InsertData();
        }
        private void InsertData()
        {
            try
            {
                var service = ServerLocation._iServiceProvider.Resolve<IExecuteSocketService>();
                //深度
                if (_msg.IndexOf("depth") > 0)
                {
                    var receiveData = JsonConvert.DeserializeObject<ReceiveDataSocket>(_msg);
                    service.ExecuteDetpthJob(receiveData);
                }
                //K线
                else if (_msg.IndexOf("kline") > 0)
                {
                }
                //trace
                else if (_msg.IndexOf("trade") > 0)
                {
                    //Trace.WriteLine(_msg);
                    var receiveData = JsonConvert.DeserializeObject<TraceDataSocket>(_msg);
                    service.ExecuteTradeJob(receiveData);
                }
            }
            catch (Exception ex)
            {
                LogManage.Error.Debug(ex);
            }

        }
    }
}
