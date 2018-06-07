using DataAnalysis.Component.Tools.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAnalysis.Component.Tools.Cache
{
    public class RedisHelper : CSRedis.QuickHelperBase
    {
        public static void InitializeConfiguration()
        {
            int port, poolsize, database;
            string ip;
            if (!int.TryParse(AppSetting.GetConnection("WebConfig", "RedisPort"), out port)) port = 8099;
            if (!int.TryParse(AppSetting.GetConnection("WebConfig", "RedisPoolSize"), out poolsize)) poolsize = 50;
            if (!int.TryParse(AppSetting.GetConnection("WebConfig", "RedisDataBase"), out database)) database = 0;
            ip = AppSetting.GetConnection("WebConfig", "RedisIP");
            Instance = new CSRedis.ConnectionPool(ip, port, poolsize);
            Instance.Connected += (s, o) =>
            {
                CSRedis.RedisClient rc = s as CSRedis.RedisClient;
                if (database > 0) rc.Select(database);
            };
        }
    }
}
