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
using DataAnalysis.Core.Data.TempEntity;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DataAnalysisFrame;
using Autofac;

namespace DataAnalysis.Application.Service.BitService
{
    public class BitBaseService
    {
        public ICacheManager _iCacheManager { get; set; }

        private string CRRENCY_NAME = "CrrencyName";
        private string CRRENCY_TYPE = "CrrencyType";
        public BitBaseService()
        {
            _iCacheManager = ServerLocation._iServiceProvider.Resolve<ICacheManager>();
        }

        public virtual void Calc(ReceiveData receiveData)
        {
            var bidsList = CalcBuyVolume(receiveData);
            var asksList = CalcSellingVolume(receiveData);
            string bidsJson = AnalysisBuyDepth(bidsList);
            string asksJson = AnalysisSellingDepth(asksList);

            var dic = GetPairs(receiveData.ch);
            string bitName = dic.Keys.Contains(CRRENCY_NAME) ? dic[CRRENCY_NAME] : string.Empty;
            InsertToRedis(bidsJson, bitName + ":buy", receiveData.tick.ts);
            InsertToRedis(asksJson, bitName + ":selling", receiveData.tick.ts);
        }

        /// <summary>
        /// 插入到redis，格式 币种:时间戳:json
        /// </summary>
        private void InsertToRedis(string json, string bitName, long tc)
        {
            _iCacheManager.Insert(string.Format("{0}:{1}", bitName, tc), json, DateTime.Now.AddMinutes(2));
        }

        /// <summary>
        /// 分析Depth买入得数据
        /// </summary>
        /// <param name="list"></param>
        private string AnalysisBuyDepth(List<DepthTempEntity> list)
        {
            /*
             * 以价格升序，分成10个区间，计算每个区间占有的值
             *  总成交量,总成交金额
             */
            var orderlist = list.OrderBy(p => p.SinglePrice);
            //总成交量
            var totalVolumn = list.Sum(p => p.SingleVolume);
            //总成交价格
            var totalPrice = list.Sum(p => p.SingleTotal);

            var sectionDic = new Dictionary<string, string>();
            //以10分为一个区间
            var section = orderlist.Count() / 15;
            int pageSize = 15;
            for (var i = 0; i < section; i++)
            {
                var tempList = orderlist.Skip(i * pageSize).Take(15).ToList();
                //取第一个和最后一个 的总价作为字典KEY
                var firstPrice = tempList.FirstOrDefault().SinglePrice;
                var lastPrice = tempList.LastOrDefault().SinglePrice;
                sectionDic.Add($"{firstPrice}-{lastPrice}", tempList.Sum(p => p.SingleTotal).ToString("0.000000"));

            }
            var res = new
            {
                totalVolumn = totalVolumn.ToString("0.0000"),
                totalPrice = totalPrice.ToString("0.000000"),
                section = sectionDic
            };
            string json = JsonConvert.SerializeObject(res);
            //Trace.WriteLine(json);
            return json;
        }

        /// <summary>
        /// 分析Depth买入得数据
        /// </summary>
        /// <param name="list"></param>
        private string AnalysisSellingDepth(List<DepthTempEntity> list)
        {
            /*
              * 以价格升序，分成10个区间，计算每个区间占有的值
              *  总成交量,总成交金额
            */
            var orderlist = list.OrderBy(p => p.SinglePrice);
            //总成交量
            var totalVolumn = list.Sum(p => p.SingleVolume);
            //总成交价格
            var totalPrice = list.Sum(p => p.SingleTotal);

            var sectionDic = new Dictionary<string, string>();
            //以10分为一个区间
            var section = orderlist.Count() / 15;
            int pageSize = 15;
            for (var i = 0; i < section; i++)
            {
                var tempList = orderlist.Skip(i * pageSize).Take(15).ToList();
                //取第一个和最后一个 的总价作为字典KEY
                var firstPrice = tempList.FirstOrDefault().SinglePrice;
                var lastPrice = tempList.LastOrDefault().SinglePrice;
                sectionDic.Add($"{firstPrice}-{lastPrice}", tempList.Sum(p => p.SingleTotal).ToString("0.000000"));

            }
            var res = new
            {
                totalVolumn = totalVolumn.ToString("0.0000"),
                totalPrice = totalPrice.ToString("0.000000"),
                section = sectionDic
            };
            string json = JsonConvert.SerializeObject(res);
            //Trace.WriteLine(json);
            return json;


        }

        /// <summary>
        /// 深度 买入量 计算每一笔得余额
        /// </summary>
        public virtual List<DepthTempEntity> CalcBuyVolume(ReceiveData receiveData)
        {
            List<DepthTempEntity> bidsList = new List<DepthTempEntity>();
            double[][] bids = receiveData.tick.bids;

            for (var i = 0; i < bids.Length; i++)
            {
                DepthTempEntity tempEntity = new DepthTempEntity();
                for (var j = 0; j < bids[i].Length; j++)
                {
                    //价格
                    if (j == 0)
                    {
                        tempEntity.SinglePrice = bids[i][j];
                    }
                    //成交量
                    else if (j == 1)
                    {
                        tempEntity.SingleVolume = bids[i][j];
                    }
                }
                //总价等于 买入价 * 买入成交
                tempEntity.SingleTotal = tempEntity.SinglePrice * tempEntity.SingleVolume;
                bidsList.Add(tempEntity);
            }
            return bidsList;
        }

        /// <summary>
        /// 深度 卖出量
        /// </summary>
        public virtual List<DepthTempEntity> CalcSellingVolume(ReceiveData receiveData)
        {
            List<DepthTempEntity> asksList = new List<DepthTempEntity>();
            double[][] asks = receiveData.tick.asks;
            for (var i = 0; i < asks.Length; i++)
            {
                DepthTempEntity tempEntity = new DepthTempEntity();
                for (var j = 0; j < asks[i].Length; j++)
                {
                    //价格
                    if (j == 0)
                    {
                        tempEntity.SinglePrice = asks[i][j];
                    }
                    //成交量
                    else if (j == 1)
                    {
                        tempEntity.SingleVolume = asks[i][j];
                    }
                }
                //总价等于 买入价 * 买入成交
                tempEntity.SingleTotal = tempEntity.SinglePrice * tempEntity.SingleVolume;
                asksList.Add(tempEntity);
            }

            return asksList;
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

