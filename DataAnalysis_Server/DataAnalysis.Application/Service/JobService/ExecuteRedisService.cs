using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Component.Tools.Cache;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Core.Data.Entity.DepthEntity;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Application.Service.JobService
{
    public class ExecuteRedisService : IExecuteRedisService
    {
        private readonly ICacheManager _iCacheManager;
        public ExecuteRedisService(ICacheManager iCacheManager)
        {
            _iCacheManager = iCacheManager;
        }


        private static object objDepthLock = new object();
        private static bool isDepthFlg = true;

        public void ExecuteDetpthRedisJob()
        {
            lock (objDepthLock)
            {
                if (isDepthFlg)
                {
                    isDepthFlg = false;
                    try
                    {
                        //查询所有的key，根据key在查询value
                        var bitDic = ReflectionHelper.GetStaticPropertyNameAndValue(typeof(BitSpecies));
                        var allDic = new Dictionary<string, DepthAnalysisEntity>();
                        foreach (KeyValuePair<string, object> keyVal in bitDic)
                        {
                            //取出当前币种对应 的key
                            var keysList = _iCacheManager.GetKeys($"{keyVal.Value}:*");
                            keysList.ForEach(p =>
                            {
                                string value = Convert.ToString(_iCacheManager.Get(p));
                                DepthAnalysisEntity entity = JsonConvert.DeserializeObject<DepthAnalysisEntity>(value);
                                if (entity == null)
                                {
                                    Trace.WriteLine($"entity为空 key:{p}");
                                }
                                allDic.Add(p, entity);
                            });
                            _iCacheManager.RemoveBatch($"{keyVal.Value}:*");
                        }
                        if (allDic.Count > 0)
                        {
                            foreach (KeyValuePair<string, DepthAnalysisEntity> keyDepth in allDic)
                            {
                                Trace.WriteLine($"{keyDepth.Value.CurrencyName}:{keyDepth.Value.ForecastAmount}");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogManage.Job.Error($"ExecuteDetpthRedisJob执行报错:{ex}");
                    }
                    finally
                    {
                        isDepthFlg = true;
                    }
                }
            }
        }

    }
}
