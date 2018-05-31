using DataAnalysis.Component.Tools.Cache;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Dynamic;
using DataAnalysis.Core.Data.BitEntity;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DataAnalysisFrame;
using Autofac;
using DataAnalysis.Core.Data.Entity.DepthEntity;
using DataAnalysis.Component.Tools.Log;

namespace DataAnalysis.Application.Service.BitService
{
    public class BitBaseService
    {
        public ICacheManager _iCacheManager { get; set; }

        private string CRRENCY_NAME = "CrrencyName";
        private string CRRENCY_TYPE = "CrrencyType";
        private List<BitDetail> bidsList { get; set; }
        private List<BitDetail> asksList { get; set; }

        public BitBaseService()
        {
            _iCacheManager = ServerLocation._iServiceProvider.Resolve<ICacheManager>();
            bidsList = new List<BitDetail>();
            asksList = new List<BitDetail>();
        }

        public virtual void Calc(ReceiveData receiveData)
        {
            CalcBuyVolume(receiveData);
            CalcSellingVolume(receiveData);
            AnalysisDepth();

            var dic = GetPairs(receiveData.ch);
            string bitName = dic.Keys.Contains(CRRENCY_NAME) ? dic[CRRENCY_NAME] : string.Empty;
            //InsertToRedis(json, bitName, receiveData.tick.ts);
        }

        /// <summary>
        /// 插入到redis，格式 币种:时间戳:json
        /// </summary>
        private void InsertToRedis(string json, string bitName, long tc)
        {
            _iCacheManager.Insert(string.Format("{0}:{1}", bitName, tc), json, DateTime.Now.AddMinutes(2));
        }

        private void CalcBuyVolume(ReceiveData receiveData)
        {
            if (receiveData != null)
            {
                double[][] bids = receiveData.tick.bids;
                for (var i = 0; i < bids.Length; i++)
                {
                    BitDetail bitEntity = new BitDetail();
                    for (var j = 0; j < bids[i].Length; j++)
                    {
                        //价格
                        if (j == 0)
                        {
                            bitEntity.Price = bids[i][j];
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

        private void CalcSellingVolume(ReceiveData receiveData)
        {
            if (receiveData != null)
            {
                double[][] asks = receiveData.tick.asks;
                for (var i = 0; i < asks.Length; i++)
                {
                    BitDetail bitEntity = new BitDetail();
                    for (var j = 0; j < asks[i].Length; j++)
                    {
                        //价格
                        if (j == 0)
                        {
                            bitEntity.Price = asks[i][j];
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
                Trace.WriteLine(JsonConvert.SerializeObject(bidsList));
                Trace.WriteLine(JsonConvert.SerializeObject(asksList));
                int bidsIndex = 0, asksIndex = 0;
                if (asksList[asksIndex].Price > bidsList[bidsIndex].Price)
                {
                    Trace.WriteLine(bidsList[bidsIndex].Price);
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
                    Trace.WriteLine(bidsList[bidsIndex >= bidsList.Count ? bidsIndex - 1 : bidsIndex].Price);
                    return;
                }
                if (asksList[asksIndex].Price > bidsList[bidsIndex].Price)
                {
                    Trace.WriteLine(bidsList[bidsIndex].Price);
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
                LogManage.Job.Debug(ex);
                Trace.WriteLine($"报错:bidsIndex:{bidsIndex},asksIndex{asksIndex}");
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
    }
}

