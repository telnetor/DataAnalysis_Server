using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Component.Tools.Cache;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Log;
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
                        foreach (KeyValuePair<string, object> keyVal in bitDic)
                        {
                            var buysValueList = new List<dynamic>();
                            var sellingValueList = new List<dynamic>();
                            //取出当前币种对应 的key
                            var buyKeysList = _iCacheManager.GetKeys($"{keyVal.Value}:buy:*");
                            var sellingKeysList = _iCacheManager.GetKeys($"{keyVal.Value}:selling:*");
                            buyKeysList.ForEach(p =>
                            {
                                string value = Convert.ToString(_iCacheManager.Get(p));
                                var array = p.Split(':');
                                dynamic dymBuys = new ExpandoObject();
                                dymBuys.bit = array[0];
                                dymBuys.type = array[1];
                                dymBuys.tc = array[2];
                                dymBuys.json = value;
                                buysValueList.Add(dymBuys);
                                _iCacheManager.Remove(p);
                            });

                            sellingKeysList.ForEach(p =>
                            {
                                string value = Convert.ToString(_iCacheManager.Get(p));
                                var array = p.Split(':');
                                dynamic dymBuys = new ExpandoObject();
                                dymBuys.bit = array[0];
                                dymBuys.type = array[1];
                                dymBuys.tc = array[2];
                                dymBuys.json = value;
                                sellingValueList.Add(dymBuys);
                                _iCacheManager.Remove(p);
                            });
                            var res = from buysItem in buysValueList
                                      join
                                      sellingItem in sellingValueList
                                      on buysItem.tc equals sellingItem.tc
                                      orderby buysItem.tc, sellingItem.tc
                                      select new
                                      {
                                          bit = buysItem.bit,
                                          type = buysItem.type,
                                          tc = buysItem.tc,
                                          buyJson = buysItem.json,
                                          sellingJson = sellingItem.json
                                      };
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
