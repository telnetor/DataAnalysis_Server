using DataAnalysis.Component.Tools.Cache;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Dynamic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DataAnalysisFrame;
using Autofac;
using DataAnalysis.Core.Data.Entity.DepthEntity;
using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Core.Data.Entity.TradeEntity;

namespace DataAnalysis.Application.Service.BitService
{
    public class BitBaseService
    {

        private string CRRENCY_NAME = "CrrencyName";
        private string CRRENCY_TYPE = "CrrencyType";
        private int timeOut = 1800;

        //预测金额
        private decimal forecastAmount { get; set; }

        private List<BitDetailEntity> bidsList=new List<BitDetailEntity>();
        private List<BitDetailEntity> asksList = new List<BitDetailEntity>();

        public BitBaseService()
        {
        }

        #region 分析深度

        public virtual void Calc(ReceiveDataSocket receiveData)
        {

            CalcBuyVolume(receiveData);
            CalcSellingVolume(receiveData);

            AnalysisDepth();

            if (forecastAmount > 0)
            {
                var dic = GetPairs(receiveData.ch);
                AnalysisBuyPrice(dic[CRRENCY_NAME]);
                string currencyName = dic.Keys.Contains(CRRENCY_NAME) ? dic[CRRENCY_NAME] : string.Empty;
                //当深度有变化才进行插入操作
                if (!HuoBiContract.depthDic.Keys.Contains(currencyName))
                {
                    var bitTarget = new
                    {
                        CurrencyName = currencyName,
                        ForecastAmount = forecastAmount,
                        ServerReturnTime = receiveData.tick.ts,
                        ForecastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
                    };
                    HuoBiContract.depthDic.Add(currencyName, forecastAmount);
                    string json = JsonConvert.SerializeObject(bitTarget);
                    //Trace.WriteLine(json);
                    InsertToRedis(json, currencyName, receiveData.tick.ts);
                }
                else
                {
                    decimal value = HuoBiContract.depthDic[currencyName];
                    if (value != forecastAmount)
                    {
                        var bitTarget = new
                        {
                            CurrencyName = currencyName,
                            ForecastAmount = forecastAmount,
                            ServerReturnTime = receiveData.tick.ts,
                            ForecastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")
                        };
                        //更新
                        HuoBiContract.depthDic[currencyName] = forecastAmount;
                        string json = JsonConvert.SerializeObject(bitTarget);
                        //Trace.WriteLine(json);
                        InsertToRedis(json, currencyName, receiveData.tick.ts);
                    }
                }

            }
        }

        /// <summary>
        /// 推算买入价格
        /// </summary>
        private void AnalysisBuyPrice(string bitName)
        {
            //_iCacheManager.Get(bitName)
            string json = Convert.ToString(RedisHelper.Get(bitName));
            if (!string.IsNullOrWhiteSpace(json))
            {
                TraceDataEntity entity = JsonConvert.DeserializeObject<TraceDataEntity>(json);
                //当前行情价小于下一轮预估的价格，则上涨
                if (entity.Price < forecastAmount)
                {
                    decimal range = forecastAmount - entity.Price;
                    Trace.WriteLine($"预测:{forecastAmount},目前:{entity.Price},涨幅-{bitName}:{range}");
                }
                //下跌
                else if (entity.Price > forecastAmount)
                {
                    decimal range = entity.Price - forecastAmount;
                    Trace.WriteLine($"预测:{forecastAmount},目前:{entity.Price},跌幅-{bitName}:{range}");
                }
                else
                {

                }
            }
        }
        /// <summary>
        /// 插入到redis，格式 币种:时间戳:json
        /// </summary>
        private void InsertToRedis(string json, string bitName, long tc)
        {
            RedisHelper.SetAsync(string.Format("{0}:{1}", bitName, tc), json, timeOut);
        }

        private void CalcBuyVolume(ReceiveDataSocket receiveData)
        {
            if (receiveData != null)
            {
                double[][] bids = receiveData.tick.bids;
                for (var i = 0; i < bids.Length; i++)
                {
                    BitDetailEntity bitEntity = new BitDetailEntity();
                    for (var j = 0; j < bids[i].Length; j++)
                    {
                        //价格
                        if (j == 0)
                        {
                            bitEntity.Price =decimal.Parse(bids[i][j].ToString());
                        }
                        //成交量
                        else if (j == 1)
                        {
                            bitEntity.Number = bids[i][j];
                        }
                    }
                    bidsList.Add(bitEntity);
                }
            }
        }

        private void CalcSellingVolume(ReceiveDataSocket receiveData)
        {
            if (receiveData != null)
            {
                double[][] asks = receiveData.tick.asks;
                for (var i = 0; i < asks.Length; i++)
                {
                    BitDetailEntity bitEntity = new BitDetailEntity();
                    for (var j = 0; j < asks[i].Length; j++)
                    {
                        //价格
                        if (j == 0)
                        {
                            bitEntity.Price = decimal.Parse(asks[i][j].ToString());
                        }
                        //成交量
                        else if (j == 1)
                        {
                            bitEntity.Number = asks[i][j];
                        }
                    }
                    asksList.Add(bitEntity);
                }

            }
        }
        /// <summary>
        /// 分析Depth买入得数据
        /// </summary>
        /// <param name="list"></param>
        private void AnalysisDepth()
        {
            if (bidsList.Count > 0 && asksList.Count > 0)
            {
                //买入按降序排序
                bidsList = bidsList.OrderByDescending(p => p.Price).ThenByDescending(c => c.Number).ToList();
                //卖出按升序排序
                asksList = asksList.OrderBy(p => p.Price).ThenByDescending(c => c.Number).ToList();
                int bidsIndex = 0, asksIndex = 0;
                if (asksList[asksIndex].Price > bidsList[bidsIndex].Price)
                {
                    forecastAmount = bidsList[bidsIndex].Price;
                    return;
                }
                double num = bidsList[bidsIndex].Number - asksList[asksIndex].Number;
                if (num > 0)
                {
                    asksIndex++;
                }
                else if (num < 0)
                {
                    bidsIndex++;
                }
                else
                {
                    asksIndex++;
                    bidsIndex++;
                }
                Func(num, bidsIndex, asksIndex);
            }

        }
        private void Func(double x, int bidsIndex, int asksIndex)
        {
            try
            {
                if ((bidsIndex >= bidsList.Count || asksIndex >= asksList.Count))
                {
                    forecastAmount = bidsList[bidsIndex >= bidsList.Count ? bidsIndex - 1 : bidsIndex].Price;
                    return;
                }
                if (asksList[asksIndex].Price > bidsList[bidsIndex].Price)
                {
                    forecastAmount = bidsList[bidsIndex].Price;
                    return;
                }
                //买入量小于卖出量，那么买入量的索引递增1
                if (x < 0 && bidsList[bidsIndex].Price >= asksList[asksIndex].Price)
                {
                    x = bidsList[bidsIndex].Number + x;
                }
                //买入量大于卖出量
                else if (x > 0 && bidsList[bidsIndex].Price >= asksList[asksIndex].Price)
                {
                    x = -asksList[asksIndex].Number + x;
                }
                else if (x == 0 && bidsList[bidsIndex].Price >= asksList[asksIndex].Price)
                {
                    x = bidsList[bidsIndex].Number - asksList[asksIndex].Number;
                }
                if (x > 0)
                {
                    asksIndex++;
                }
                else if (x < 0)
                {
                    bidsIndex++;
                }
                else
                {
                    asksIndex++;
                    bidsIndex++;
                }
                Func(x, bidsIndex, asksIndex);
            }
            catch (Exception ex)
            {
                LogManage.Job.Debug($"{ex}----bidsIndex:{bidsIndex},asksIndex{asksIndex}");
            }
        }


        public Dictionary<string, string> GetPairs(string ch)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(ch))
            {
                var array = ch.Split('.');
                if (array.Length >= 4)
                {
                    if (!dic.Keys.Contains(CRRENCY_NAME))
                    {
                        dic.Add(CRRENCY_NAME, array[1].Replace("usdt", string.Empty));
                    }
                    if (!dic.Keys.Contains(CRRENCY_TYPE))
                    {
                        dic.Add(CRRENCY_TYPE, array[2]);
                    }
                }
            }
            return dic;
        }

        #endregion

        #region 当前成交量
        public void AnalysisTrade(TraceDataSocket receiveData)
        {
            Dictionary<string, string> dic = GetPairs(receiveData.ch);
            //取data索引为0，为最新一条 
            int len = receiveData.tick.data.Length - 1;
            DateTime dt = UtilsHelper.GetTime(long.Parse(receiveData.tick.data[len].
                datumTs.ToString().Substring(0, 10)));
            var obj = new
            {
                CurrencyName = dic[CRRENCY_NAME],
                TransactionDate= dt.ToString("yyyy-MM-dd HH:mm:ss:ff"),
                Amount= receiveData.tick.data[len].amount,
                Price = receiveData.tick.data[len].price,
                Direction= receiveData.tick.data[0].direction
            };
            string json = JsonConvert.SerializeObject(obj);
            RedisHelper.SetAsync(obj.CurrencyName, json, timeOut);
        }
        #endregion
    }
}

