using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DataAnalysis.Manipulation.DapperExtension
{
    public class DbProviderFactories
    {
        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            if (string.IsNullOrEmpty(providerInvariantName)) throw new Exception("数据库链接字符串配置不正确！");
            if (providerInvariantName.ToLower().Contains("mysql"))
            {
                return new MySqlClientFactory();
            }
            throw new Exception("暂不支持您使用的数据库类型！");
        }

    }
}
