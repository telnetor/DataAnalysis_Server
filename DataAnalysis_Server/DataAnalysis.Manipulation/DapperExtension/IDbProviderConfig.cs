using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Manipulation.DapperExtension
{
    public interface IDbProviderConfig
    {
        /// <summary>
        /// 数据库驱动提供程序 1.MySql: MySql.Data.MySqlClient 2.MsSql: System.Data.SqlClient
        /// </summary>
        string ProviderFactoryString { get; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string DbConnectionString { get; }
    }
}
