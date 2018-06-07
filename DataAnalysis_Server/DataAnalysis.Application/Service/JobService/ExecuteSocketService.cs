using Autofac;
using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Application.Service.BitService;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using DataAnalysis.Component.Tools.Log;
using DataAnalysisFrame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalysis.Application.Service.JobService
{
    /// <summary>
    /// 定时执行队列
    /// </summary>
    public class ExecuteSocketService : IExecuteSocketService
    {


        private static object objDepthLock = new object();
        private static bool isDepthFlg = true;

        private static object objTradeLock = new object();
        private static bool isTradeFlg = true;

        public void ExecuteDetpthJob(ReceiveDataSocket receiveData)
        {
            lock (objDepthLock)
            {
                if (isDepthFlg)
                {
                    isDepthFlg = false;
                    try
                    {
                        var service=ServerLocation._iServiceProvider.Resolve<BitBaseService>();
                        service.Calc(receiveData);
                    }
                    catch (Exception ex)
                    {
                        LogManage.Job.Error($"ExecuteDetpthQueueJob执行报错:{ex}");
                    }
                    finally
                    {
                        isDepthFlg = true;
                    }
                }
            }
        }

        public void ExecuteKLineJob()
        {
            throw new NotImplementedException();
        }


        public void ExecuteTradeJob(TraceDataSocket receiveData)
        {
            lock (objTradeLock)
            {
                if (isTradeFlg)
                {
                    isTradeFlg = false;
                    try
                    {
                        var service = ServerLocation._iServiceProvider.Resolve<BitBaseService>();
                        service.AnalysisTrade(receiveData);
                    }
                    catch (Exception ex)
                    {
                        LogManage.Job.Error($"ExecuteDetpthQueueJob执行报错:{ex}");
                    }
                    finally
                    {
                        isTradeFlg = true;
                    }
                }
            }
        }
    }
}
