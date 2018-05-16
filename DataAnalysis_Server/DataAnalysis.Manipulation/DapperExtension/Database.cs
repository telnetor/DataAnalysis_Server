using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Manipulation.DapperExtension
{
    public class Database
    {
        private static DbProviderFactory _df = null;
        /// <summary>  
        /// 创建工厂提供器并且  
        /// </summary>  
        public static IDbConnection DbService(IDbProviderConfig config)
        {
            if (_df == null)
                _df=DbProviderFactories.GetFactory(config.ProviderFactoryString);
            var connection = _df.CreateConnection();
            if (connection == null) throw new Exception("创建IDbConnection异常");

            connection.ConnectionString = config.DbConnectionString;
            //if (connection.State != ConnectionState.Open) connection.Open();
            return connection;

        }
    }
}
