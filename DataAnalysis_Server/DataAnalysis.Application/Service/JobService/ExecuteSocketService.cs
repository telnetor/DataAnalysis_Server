using Autofac;
using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Application.Service.BitService;
using DataAnalysis.Component.Tools.Constant;
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

        private static object objLineLock = new object();
        private static bool isLineFlg = true;

        public void ExecuteDetpthJob(ReceiveData receiveData)
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

        public void ExecuteKLineQueueJob()
        {
            throw new NotImplementedException();
        }
    }
}
