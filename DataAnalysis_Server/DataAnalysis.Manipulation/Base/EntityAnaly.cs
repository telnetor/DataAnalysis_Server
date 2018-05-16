
using System;
using System.Collections.Generic;

namespace DataAnalysis.Manipulation.Base
{
    /// <summary>
    /// Model实体的解析接口
    /// </summary>
    public abstract class EntityAnaly
    {
        static readonly object LockObj1 = new object();
        static readonly object LockObj2 = new object();

        static readonly object LockTab = new object();

        /// <summary>
        /// 实体类缓存,静态变量是保存为了减少反射次数
        /// </summary>
        static Dictionary<Type, Dictionary<string, FieldAttribute>> _entityAttributeCache;
        static Dictionary<Type, string> _tableAttributeCache;

        /// <summary>
        /// 实体类缓存,静态变量是保存为了减少反射次数
        /// </summary>
        protected Dictionary<Type, Dictionary<string, FieldAttribute>> EntityAttributeCache
        {
            get
            {
                if (_entityAttributeCache == null)
                {
                    lock (LockObj1)
                    {
                        if (_entityAttributeCache == null)
                        {
                            _entityAttributeCache = new Dictionary<Type, Dictionary<string, FieldAttribute>>();
                        }
                    }
                }
                return _entityAttributeCache;
            }
        }

        /// <summary>
        /// 实体类缓存,静态变量是保存为了减少反射次数
        /// </summary>
        protected Dictionary<Type, string> TabelAttributeCache
        {
            get
            {
                if (_tableAttributeCache == null)
                {
                    lock (LockObj1)
                    {
                        if (_tableAttributeCache == null)
                        {
                            _tableAttributeCache = new Dictionary<Type, string>();
                        }
                    }
                }
                return _tableAttributeCache;
            }
        }
        /// <summary>
        /// 获取Model的属性对象,获取第一次后会放入一个缓存列表中
        /// 即只反射一次
        /// </summary>
        public Dictionary<string, FieldAttribute> GetEntityAttribute<T>() where T : BaseEntity
        {
            Type t = typeof(T);
            if (!EntityAttributeCache.ContainsKey(t))
            {
                lock (LockObj2)
                {
                    if (!EntityAttributeCache.ContainsKey(t))
                    {
                        var attrs = GetModelParam<T>();
                        EntityAttributeCache.Add(t, attrs);
                    }
                }
            }
            return EntityAttributeCache[t];


            //Type t = typeof(T);
            //if (EntityAttributeCache.ContainsKey(t))
            //{
            //    return EntityAttributeCache[t];
            //}
            //var attrs = GetModelParam<T>();
            //EntityAttributeCache.Add(t, attrs);
            //return attrs;
        }

        public string GetTableName<T>() where T : BaseEntity
        {
            Type t = typeof(T);
            if (!TabelAttributeCache.ContainsKey(t))
            {
                lock (LockTab)
                {
                    if (!TabelAttributeCache.ContainsKey(t))
                    {
                        var attrs = GetModelName<T>();
                        TabelAttributeCache.Add(t, attrs);
                    }
                }
            }
            return TabelAttributeCache[t];
        }

        /// <summary>
        /// 通过解析获得Model的对象的参数,Key:为类的属性名
        /// </summary>
        /// <returns>返回model参数</returns>
        protected abstract Dictionary<string, FieldAttribute> GetModelParam<T>() where T : BaseEntity;
        /// <summary>
        /// 根据Model类型获取表名
        /// </summary>
        protected abstract string GetModelName<T>() where T : class;
    }
}
