using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Component.Tools.Cache;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Core.Data.Entity.DepthEntity;
using DataAnalysis.Core.Data.IRepositories.IDepthRepositories;
using Newtonsoft.Json;
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
        private readonly IDepthRepository _iDepthRepository;

        public ExecuteRedisService(IDepthRepository iDepthRepository)
        {
            _iDepthRepository = iDepthRepository;
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
                        var addList = new List<DepthAnalysisEntity>();
                        foreach (KeyValuePair<string, object> keyVal in bitDic)
                        {
                            //取出当前币种对应 的key
                            var keysList = RedisHelper.Keys($"{keyVal.Value}:*").ToList();
                            //var keysList = _iCacheManager.GetKeys($"{keyVal.Value}:*");
                            keysList.ForEach(p =>
                            {
                                string value = Convert.ToString(RedisHelper.Get(p));
                                DepthAnalysisEntity entity = JsonConvert.DeserializeObject<DepthAnalysisEntity>(value);
                                if (entity != null)
                                {
                                    addList.Add(entity);
                                }
                                RedisHelper.Remove(p);
                            });
                            
                            //RedisHelper.RemoveBatch($"{keyVal.Value}:*");
                        }
                        //_iDepthRepository.AddBulk<List<DepthAnalysisEntity>>(addList);
                        //_iDepthRepository.Add<DepthAnalysisEntity>(addList[0]);
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
