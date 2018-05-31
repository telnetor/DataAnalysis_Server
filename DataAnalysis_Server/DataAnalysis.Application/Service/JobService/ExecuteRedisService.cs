using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Component.Tools.Cache;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Log;
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
                        var allDic = new Dictionary<string, dynamic>();
                        foreach (KeyValuePair<string, object> keyVal in bitDic)
                        {
                            //取出当前币种对应 的key
                            var keysList = _iCacheManager.GetKeys($"{keyVal.Value}:*");
                            keysList.ForEach(p =>
                            {
                                string value = Convert.ToString(_iCacheManager.Get(p));
                                dynamic dyn = JsonConvert.DeserializeObject<dynamic>(value);
                                var array = p.Split(':');
                                dynamic dymBit = new ExpandoObject();
                                dymBit.bit = array[0];
                                dymBit.ts = array[1];
                                //买入金额数量
                                dymBit.buyTotalVolumn = dyn.buyTotalVolumn;
                                //买入总金额
                                dymBit.buyTotalPrice = dyn.buyTotalPrice;
                                //卖出金额数量
                                dymBit.sellingTotalVolumn = dyn.sellingTotalVolumn;
                                //卖出总数量
                                dymBit.sellingTotalPrice = dyn.sellingTotalPrice;
                                dymBit.bidsJson = dyn.bidsList;
                                dymBit.aidsJson = dyn.asksList;
                                allDic.Add(p, dymBit);
                            });
                            _iCacheManager.RemoveBatch($"{keyVal.Value}:*");
                        }
                        if (allDic.Count > 0)
                        {
                            foreach (KeyValuePair<string, dynamic> keyDepth in allDic)
                            {
                                Console.WriteLine(keyDepth.Key);
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
