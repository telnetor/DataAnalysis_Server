
using DataAnalysis.Component.Tools.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DataAnalysis.Manipulation.Base
{
    /// <summary>
    /// 使用反射来解析Model
    /// </summary>
    public class EntityAnalyReflect : EntityAnaly
    {
        /// <summary>
        /// 表名是否以复数形式表示
        /// </summary>
        readonly bool _enablePluralization;

        public EntityAnalyReflect(bool enablePluralization)
        {
            _enablePluralization = enablePluralization;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        protected override string GetModelName<TModel>()
        {

            TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(typeof(TModel), typeof(TableAttribute));
            return tableAttribute.TableName;
        }

        /// <summary>
        /// 通过解析获得Model的对象的参数,Key:为类的属性名
        /// </summary>
        /// <returns>返回model参数</returns>
        protected override Dictionary<string, FieldAttribute> GetModelParam<TModel>()
        {
            var list = new Dictionary<string, FieldAttribute>();
            PropertyInfo[] pros = ReflectionHelper.GetPropertyInfo<TModel>();
            foreach (PropertyInfo item in pros)
            {
                var attr = ReflectionHelper.GetCustomAttribute<FieldAttribute>(item);
                if (attr == null)
                {
                    //如果实体没定义属性则创建一个新的
                    attr = new FieldAttribute { ColumnName = item.Name };
                }
                else
                {
                    //如果列名没有赋值,则将列名定义和属性名一样的值
                    if (string.IsNullOrEmpty(attr.ColumnName))
                    {
                        attr.ColumnName = item.Name;
                    }
                }
                list.Add(item.Name, attr);
            }
            return list;
        }
    }
}
