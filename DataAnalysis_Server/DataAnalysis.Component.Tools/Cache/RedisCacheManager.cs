using DataAnalysis.Component.Tools.Settings;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text.RegularExpressions;

namespace DataAnalysis.Component.Tools.Cache
{
    /// <summary>
    /// 缓存实现
    /// </summary>
    public partial class RedisCacheManager : ICacheManager
    {
        private static string RedisPath = AppSetting.GetConnection("WebConfig", "RedisConfig");

        int Default_Timeout = 600;//默认超时时间（单位秒）
        string address;
        JsonSerializerSettings jsonConfig = new JsonSerializerSettings() { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };
        ConnectionMultiplexer connectionMultiplexer;
        IServer server;
        IDatabase database;

        class CacheObject<T>
        {
            public int ExpireTime { get; set; }
            public bool ForceOutofDate { get; set; }
            public T Value { get; set; }
        }

        public RedisCacheManager()
        {
            this.address = RedisPath;
            if (this.address == null || string.IsNullOrWhiteSpace(this.address.ToString()))
                throw new ApplicationException("配置文件中未找到RedisServer的有效配置");
            connectionMultiplexer = ConnectionMultiplexer.Connect(address);
            server= connectionMultiplexer.GetServer(address);
            database = connectionMultiplexer.GetDatabase();
        }

        /// <summary>
        /// 连接超时设置
        /// </summary>
        public int TimeOut
        {
            get
            {
                return Default_Timeout;
            }
            set
            {
                Default_Timeout = value;
            }
        }

        public string PrefixKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object Get(string key)
        {
            return Get<object>(key);
        }

        public T Get<T>(string key)
        {
            DateTime begin = DateTime.Now;
            var cacheValue = database.StringGet(key);
            DateTime endCache = DateTime.Now;
            var value = default(T);
            if (!cacheValue.IsNull)
            {
                var cacheObject = JsonConvert.DeserializeObject<CacheObject<T>>(cacheValue, jsonConfig);
                if (!cacheObject.ForceOutofDate)
                    database.KeyExpire(key, new TimeSpan(0, 0, cacheObject.ExpireTime));
                value = cacheObject.Value;
            }
            DateTime endJson = DateTime.Now;
            return value;

        }

        public void Insert(string key, object data)
        {
            var jsonData = GetJsonData(data, TimeOut, false);
            database.StringSet(key, jsonData);
        }

        public void Insert(string key, object data, int cacheTime)
        {
            var timeSpan = TimeSpan.FromSeconds(cacheTime);
            var jsonData = GetJsonData(data, TimeOut, true);
            database.StringSet(key, jsonData, timeSpan);
        }

        public void Insert(string key, object data, DateTime cacheTime)
        {
            var timeSpan = cacheTime - DateTime.Now;
            var jsonData = GetJsonData(data, TimeOut, true);
            database.StringSet(key, jsonData, timeSpan);
        }

        public void Insert<T>(string key, T data)
        {
            var jsonData = GetJsonData<T>(data, TimeOut, false);
            database.StringSet(key, jsonData);
        }

        public void Insert<T>(string key, T data, int cacheTime)
        {
            var timeSpan = TimeSpan.FromSeconds(cacheTime);
            var jsonData = GetJsonData<T>(data, TimeOut, true);
            database.StringSet(key, jsonData, timeSpan);
        }

        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            var timeSpan = cacheTime - DateTime.Now;
            var jsonData = GetJsonData<T>(data, TimeOut, true);
            database.StringSet(key, jsonData, timeSpan);
        }


        string GetJsonData(object data, int cacheTime, bool forceOutOfDate)
        {
            var cacheObject = new CacheObject<object>() { Value = data, ExpireTime = cacheTime, ForceOutofDate = forceOutOfDate };
            return JsonConvert.SerializeObject(cacheObject, jsonConfig);//序列化对象
        }

        string GetJsonData<T>(T data, int cacheTime, bool forceOutOfDate)
        {
            var cacheObject = new CacheObject<T>() { Value = data, ExpireTime = cacheTime, ForceOutofDate = forceOutOfDate };
            return JsonConvert.SerializeObject(cacheObject, jsonConfig);//序列化对象
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            database.KeyDelete(key, CommandFlags.HighPriority);
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        public bool Exists(string key)
        {
            return database.KeyExists(key);
        }

        /// <summary>
        /// 获取所有的Kev
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public List<string> GetKeys(string pattern="")
        {
            pattern = string.IsNullOrWhiteSpace(pattern) ? "*" : pattern;
            var keysList = new List<string>();
            foreach (var key in server.Keys(pattern: pattern))
            {
                keysList.Add(key);
            }
            return keysList;
        }

        public void RemoveBatch(string pattern = "")
        {
            pattern = string.IsNullOrWhiteSpace(pattern) ? "*" : pattern;
            foreach (var key in server.Keys(pattern: pattern))
            {
                Remove(pattern);
            }
        }
        public string AddKeyPrefix(string key)
        {
            return $"{PrefixKey}:{key}";
        }
    }
}
