using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Application.Service.BitService;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAnalysis.Application.Service.JobService
{
    /// <summary>
    /// 定时执行队列
    /// </summary>
    public class ExecuteQueueService : IExecuteQueueService
    {


        private static object objDepthLock = new object();
        private static bool isDepthFlg = true;

        private static object objLineLock = new object();
        private static bool isLineFlg = true;

        public void ExecuteDetpthQueueJob()
        {
            lock (objDepthLock)
            {
                if (isDepthFlg)
                {
                    isDepthFlg = false;
                    try
                    {
                        //先获取集合，避免循环时另外线程插入到队列
                        int queueCount = SystemContract.messageDepthQueue.Count;
                        if (queueCount > 0)
                        {
                            List<ReceiveData> list = new List<ReceiveData>();
                            for (int i = 0; i < queueCount; i++)
                            {
                                ReceiveData receiveData = new ReceiveData();
                                if (SystemContract.messageDepthQueue.TryDequeue(out receiveData))
                                {
                                    list.Add(receiveData);
                                }
                            }
                            if (list.Count > 0)
                            {
                                if (HuoBiContract.topicDic.Count > 0)
                                {
                                    foreach (KeyValuePair<string, string> keyVaule in HuoBiContract.topicDic)
                                    {
                                        var res=list.Where(p => p.ch.Equals(keyVaule.Key));
                                        if (res.Any())
                                        {
                                            var tempList = list.OrderBy(p => p.tick.ts).ToList();
                                            tempList.ForEach(p =>
                                            {
                                                BitBaseService baseService = BitFactory.GetSingle(p.ch);
                                                if (baseService != null)
                                                {
                                                    baseService.Calc(p);
                                                }
                                            });
                                        }
                                    }
                                }
                                //Trace.WriteLine($"queue之前 count:{SystemContract.messageDepthQueue.Count}");
                                //Trace.WriteLine($"list count:{list.Count}");
                                //Trace.WriteLine($"queue之后 count:{SystemContract.messageDepthQueue.Count}");
                            }
                        }
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
