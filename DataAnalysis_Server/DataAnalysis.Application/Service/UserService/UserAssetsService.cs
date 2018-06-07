using DataAnalysis.Application.IService.IUserService;
using DataAnalysis.Component.Tools.Constant.ResponseEntity;
using DataAnalysis.Component.Tools.Http;
using DataAnalysis.Component.Tools.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using DataAnalysis.Component.Tools.Constant;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Core.Data.Entity.UserAssetEntity;
using System.Dynamic;
using DataAnalysis.Core.Data.IRepositories.IUserRepositories;

namespace DataAnalysis.Application.Service.UserService
{
    public class UserAssetsService : IUserAssetsService
    {
        private readonly IBalanceRepository _iBalanceRepository;
        public UserAssetsService(IBalanceRepository iBalanceRepository)
        {
            _iBalanceRepository = iBalanceRepository;
        }
        /// <summary>
        /// 查询当前委托、历史委托
        /// </summary>
        public void GerOrderDelegate()
        {
            //GET /v1/order/orders
            string resourcePath = AppSetting.GetConnection("HuoBi", "GetOrderDelegate");
            HBResponse<OrderDelegateResponse> rsp = HttpRestHelper.SendRequestEncryption<OrderDelegateResponse>(resourcePath);
        }

        /// <summary>
        /// 查询指定账户的余额
        /// </summary>
        public void GetUserAccounts()
        {
            string resourcePath = AppSetting.GetConnection("HuoBi", "GetAccountBalance");
            string proAccountId = AppSetting.GetConnection("HuoBi", "GetProAccountId");
            resourcePath = resourcePath.Replace("{account-id}", proAccountId);
            //GET /v1/account/accounts/{account-id}/balance
            HBResponse<BalanceResponse> rsp = HttpRestHelper.SendRequestEncryption<BalanceResponse>(resourcePath);
            //trade: 交易余额，frozen: 冻结余额
            var res = from item in rsp.Data.list
                      where double.Parse(item.balance) != 0.0
                      group item by item.currency into m
                      select new BalanceEntity()
                      {
                          CurrencyName = m.Key,
                          CreateDate = DateTime.Now,
                          TradeBalance = m.Where(p => p.type.Equals("trade")).Sum(o => double.Parse(o.balance)),
                          FrozenBalance = m.Where(p => p.type.Equals("frozen")).Sum(o => double.Parse(o.balance))
                      };
            if (res.Any())
            {
                _iBalanceRepository.AddBulk<List<BalanceEntity>>(res.ToList());
            }
            

        }

        /// <summary>
        /// 查询当前用户的所有账户
        /// </summary>
        /// <returns></returns>
        public int GetUserId()
        {
            string resourcePath = AppSetting.GetConnection("HuoBi", "GetAccountId");
            //GET /v1/account/accounts
            HBResponse<AccountsResponse> rsp = HttpRestHelper.SendRequestEncryption<AccountsResponse>(resourcePath);
            if (rsp.Status.Equals("ok"))
            {
                return rsp.Data.user_id;
            }
            return 0;
        }
    }
}
