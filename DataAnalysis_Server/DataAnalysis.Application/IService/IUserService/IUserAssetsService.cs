using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Application.IService.IUserService
{
    public interface IUserAssetsService
    {
        /// <summary>
        /// 查询当前用户的所有账户
        /// </summary>
        /// <returns></returns>
        int GetUserId();

        /// <summary>
        /// 查询指定账户的余额
        /// </summary>
        void GetUserAccounts();

        /// <summary>
        /// 查询当前委托、历史委托
        /// </summary>
        void GerOrderDelegate();
    }
}
