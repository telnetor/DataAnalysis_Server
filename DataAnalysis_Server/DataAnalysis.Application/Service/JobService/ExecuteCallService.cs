using DataAnalysis.Application.IService.ICallService;
using DataAnalysis.Application.IService.IJobService;
using DataAnalysis.Application.IService.IUserService;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Core.Data.Entity.CallEntity;
using DataAnalysis.Core.Data.IRepositories.IDepthRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace DataAnalysis.Application.Service.JobService
{
    public class ExecuteCallService : IExecuteCallService
    {
        private readonly IUserAssetsService _iUserAssetsService;
        private readonly IObtainService _iObtainService;
        private readonly IMarketDetailRepository _iMarketDetailRepository;

        public ExecuteCallService(IUserAssetsService iUserAssetsService, IObtainService iObtainService,
            IMarketDetailRepository iMarketDetailRepository)
        {
            _iUserAssetsService = iUserAssetsService;
            _iObtainService = iObtainService;
            _iMarketDetailRepository = iMarketDetailRepository;
        }
        /// <summary>
        /// 每天0点准时获取金额
        /// </summary>
        public void ExecuteBalanceJob()
        {
            try
            {
                _iUserAssetsService.GetUserAccounts();
            }
            catch (Exception ex)
            {
                LogManage.Job.Error($"ExecuteBalanceJob执行报错:{ex}");
            }
        }

        /// <summary>
        /// 每天0点获取24小时成交量数据
        /// </summary>
        public void ExecuteMarketDetailJob()
        {
            try
            {
                var dicBit = ReflectionHelper.GetStaticPropertyNameAndValue(typeof(BitSpecies));
                if (dicBit != null && dicBit.Count > 0)
                {
                    List<MarketDetailEntity> list = new List<MarketDetailEntity>();
                    foreach (KeyValuePair<string, object> obj in dicBit)
                    {
                        var res = _iObtainService.GetMmarketDetail(Convert.ToString(obj.Value));
                        if (res != null)
                        {
                            MarketDetailEntity entity = new MarketDetailEntity();
                            entity.CurrencyName = Convert.ToString(obj.Value);
                            entity.CreateDate = DateTime.Now;
                            entity.Amount = res.tick.amount;
                            entity.Open = res.tick.open;
                            entity.Close = res.tick.close;
                            entity.High = res.tick.high;
                            entity.Ts = res.tick.ts;
                            entity.MarketId = res.tick.id;
                            entity.Count = res.tick.count;
                            entity.Low = res.tick.low;
                            entity.Vol = res.tick.vol;
                            list.Add(entity);
                        }

                    }
                    if (list.Count > 0)
                    {
                        _iMarketDetailRepository.AddBulk<List<MarketDetailEntity>>(list);
                        SendMarketDetailToEmail(list);
                    }
                }

            }
            catch (Exception ex)
            {
                LogManage.Job.Error($"ExecuteSendMailJob执行报错:{ex}");
            }
        }

        private void SendMarketDetailToEmail(List<MarketDetailEntity> list)
        {
            StringBuilder sb = new StringBuilder();
            string title = $"{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}:成交量报表";
            list.ForEach(p =>
            {
                sb.AppendLine($"币种:{p.CurrencyName}");
                sb.AppendLine($"24小时成交量:{p.Amount}");
                sb.AppendLine($"前推24小时成交价:{p.Open}");
                sb.AppendLine($"当前成交价:{p.Close}");
                sb.AppendLine($"近24小时最高价:{p.High}");
                sb.AppendLine($"近24小时最低价:{p.Low}");
                sb.AppendLine($"近24小时累积成交数:{p.Low}");
                sb.AppendLine($"近24小时累积成交额:{p.Vol}");
                sb.AppendLine(Environment.NewLine);
            });
            EmailHelper.SendEmail(title, sb.ToString());
        }
    }
}
