using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DataAnalysis.Component.Tools.Common
{
    /// <summary>
    /// 反射操作的封装
    /// </summary>
    public class ReflectionHelper
    {
        /// <summary>
        /// 获得对象的所有公共属性信息
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">获得的对象</param>
        /// <returns>返回属性信息</returns>
        public static PropertyInfo[] GetPropertyInfo<T>() where T : class
        {
            Type t = typeof(T);
            PropertyInfo[] proInfo = t.GetProperties();
            return proInfo;
        }
        /// <summary>
        /// 根据属性名获取属性信息
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">源对象</param>
        /// <param name="pName">属性名称</param>
        /// <returns>返回属性类型对象</returns>
        public static PropertyInfo GetPropertyInfo<T>(string pName) where T : class
        {
            Type t = typeof(T);

            PropertyInfo proInfo = t.GetProperty(pName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            return proInfo;
        }
        /// <summary>
        /// 获得对象的所有公共属性的属性名
        /// </summary>        
        public static string[] GetPropertyNames<T>() where T : class
        {
            PropertyInfo[] pInfos = GetPropertyInfo<T>();
            if (pInfos != null)
            {
                List<string> pNamesList = new List<string>();
                foreach (PropertyInfo item in pInfos)
                {
                    pNamesList.Add(item.Name);
                }
                return pNamesList.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 根据对象类型获取所有公共属性名
        /// </summary>
        public static string[] GetPropertyNames(Type objType)
        {
            PropertyInfo[] pInfos = objType.GetProperties();
            if (pInfos != null)
            {
                List<string> pNamesList = new List<string>();
                foreach (PropertyInfo item in pInfos)
                {
                    pNamesList.Add(item.Name);
                }
                return pNamesList.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 获得对象的属性名和属性值
        /// </summary>
        public static Dictionary<string, object> GetPropertyNameAndValue<T>(T obj) where T : class
        {
            PropertyInfo[] pInfos = GetPropertyInfo<T>();
            if (pInfos != null)
            {
                Dictionary<string, object> pNameAndValue = new Dictionary<string, object>();
                foreach (PropertyInfo item in pInfos)
                {
                    string pName = item.Name;
                    object pValue = item.GetValue(obj, null);
                    pNameAndValue.Add(item.Name, pValue);
                }
                return pNameAndValue;
            }
            return null;
        }
        /// <summary>
        /// 根据属性名设置对象的 属性值
        /// </summary>
        public static void SetPropertyValue<T>(T obj, Dictionary<string, object> pNameAndValue) where T : class, new()
        {
            foreach (string item in pNameAndValue.Keys)
            {
                if (string.IsNullOrEmpty(item)) continue;
                PropertyInfo pInfo = GetPropertyInfo<T>(item);
                if (pInfo != null)
                {
                    object pValue = pNameAndValue[item];
                    if (pValue != null)
                    {
                        object newValue = Convert.ChangeType(pValue, pInfo.PropertyType);
                        //object newValue = pValue;
                        pInfo.SetValue(obj, newValue, null);
                    }
                }
            }
        }
        /// <summary>
        /// 根据一个DataRow对象设置对象的属性值
        /// </summary>
        /// <typeparam name="T">可实例化的一个类</typeparam>
        /// <param name="dr">数据航</param>
        /// <returns>返回T</returns>
        public static void SetPropertyValue<T>(T obj, DataRow dr) where T : class, new()
        {
            //if (dr == null) return;
            //if (dr.Table.Columns.Count == 0) return;
            Dictionary<string, object> pNameAndValue = new Dictionary<string, object>();
            foreach (DataColumn item in dr.Table.Columns)
            {
                if (dr[item.ColumnName] != DBNull.Value)
                    pNameAndValue.Add(item.ColumnName, dr[item.ColumnName]);
                //if (string.IsNullOrEmpty(item.ColumnName)) continue;
                //PropertyInfo pInfo = GetPropertyInfo<T>(item.ColumnName);
                //if (pInfo == null) continue;
                //object pValue = dr[item.ColumnName];
                //pInfo.SetValue(t, pValue, null);
            }
            SetPropertyValue(obj, pNameAndValue);
        }

        /// <summary>
        /// 根据类型设置对象的默认属性值
        /// </summary>
        public static void SetPropertyValue<T>(T obj, Dictionary<Type, object> pTypeAndValue) where T : class, new()
        {
            PropertyInfo[] pInfos = GetPropertyInfo<T>();
            if (pInfos == null) return;
            foreach (PropertyInfo item in pInfos)
            {
                Type itemType = item.PropertyType;
                if (pTypeAndValue.ContainsKey(itemType))
                {
                    //object newValue = Convert.ChangeType(pValue, pInfo.PropertyType);
                    item.SetValue(obj, pTypeAndValue[itemType], null);
                }
            }
        }

        /// <summary>
        /// 根据类型设置对象的空值的默认值
        /// </summary>
        public static void SetPropertyNullValue<T>(T obj, Dictionary<Type, object> pTypeAndValue) where T : class, new()
        {
            PropertyInfo[] pInfos = GetPropertyInfo<T>();
            if (pInfos == null) return;
            foreach (PropertyInfo item in pInfos)
            {
                Type itemType = item.PropertyType;
                object attrValue = item.GetValue(obj, null);

                if (pTypeAndValue.ContainsKey(itemType))
                {
                    if (attrValue == null)
                    { }
                    else if (itemType == typeof(DateTime) && Convert.ToDateTime(attrValue) == new DateTime())
                    {
                        item.SetValue(obj, pTypeAndValue[itemType], null);
                    }
                    else if (itemType == typeof(Guid) && new Guid(attrValue.ToString()) == Guid.Empty)
                    {
                        item.SetValue(obj, pTypeAndValue[itemType], null);
                    }
                }

            }
        }
        /// <summary>
        /// 反射创建一个对象
        /// </summary>
        /// <typeparam name="T">要创建对象的类型</typeparam>
        /// <param name="assemblyName">项目名称(程序集名称,dll文件名)</param>
        /// <param name="typeName">类型名称,类的类型</param>
        /// <returns>返回一个T对象</returns>
        public static T CreateInstanc<T>(string assemblyName, string typeName) where T : class, new()
        {
            //程序集名称(项目名)
            //string assemblyName = "";
            Assembly assembly = Assembly.Load(assemblyName);
            //类型名称,完整命名空间
            //string typeName = "";
            Type type = assembly.GetType(typeName, true, true);
            object modelObj = Activator.CreateInstance(type);
            T rObj = modelObj as T;
            return rObj;
        }
        /// <summary>
        /// 获得指定成员的特性对象
        /// </summary>
        /// <typeparam name="T">要获取属性的类型</typeparam>
        /// <param name="pInfo">属性原型</param>
        /// <returns>返回T对象</returns>
        public static T GetCustomAttribute<T>(PropertyInfo pInfo) where T : Attribute, new()
        {
            Type attributeType = typeof(T);
            Attribute attrObj = Attribute.GetCustomAttribute(pInfo, attributeType);
            T rAttrObj = attrObj as T;
            return rAttrObj;
        }


    }
    public class ReflectionHelper<T> where T : class
    {
        /// <summary>
        /// 获得对象的所有公共属性
        /// </summary>
        public PropertyInfo[] GetPropertyInfo(T obj)
        {
            Type t = obj.GetType();
            PropertyInfo[] proInfo = t.GetProperties();
            return proInfo;
        }

        public PropertyInfo GetPropertyInfo(T obj, string pName)
        {
            Type t = obj.GetType();
            PropertyInfo proInfo = t.GetProperty(pName, BindingFlags.IgnoreCase);
            return proInfo;
        }

        /// <summary>
        /// 获得对象的所有公共属性的属性名
        /// </summary>        
        public string[] GetPropertyNames(T obj)
        {
            PropertyInfo[] pInfos = GetPropertyInfo(obj);
            if (pInfos != null)
            {
                List<string> pNamesList = new List<string>();
                foreach (PropertyInfo item in pInfos)
                {
                    pNamesList.Add(item.Name);
                }
                return pNamesList.ToArray();
            }
            return null;
        }
        /// <summary>
        /// 获得对象的属性名和属性值
        /// </summary>
        public Dictionary<string, object> GetPropertyNameAndValu(T obj)
        {
            PropertyInfo[] pInfos = GetPropertyInfo(obj);
            if (pInfos != null)
            {
                Dictionary<string, object> pNameAndValue = new Dictionary<string, object>();
                foreach (PropertyInfo item in pInfos)
                {
                    string pName = item.Name;
                    object pValue = item.GetValue(obj, null);
                    pNameAndValue.Add(item.Name, pValue);
                }
                return pNameAndValue;
            }
            return null;
        }
        /// <summary>
        /// 设置对象的 属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void SetPropertyValue(T obj, Dictionary<string, object> pNameAndValue)
        {
            foreach (string item in pNameAndValue.Keys)
            {
                if (string.IsNullOrEmpty(item)) continue;
                PropertyInfo pInfo = GetPropertyInfo(obj, item);
                if (pInfo != null)
                {
                    object pValue = pNameAndValue[item];
                    if (pValue != null)
                    {
                        //object newValue = Convert.ChangeType(pValue, pInfo.PropertyType);
                        object newValue = pValue;
                        pInfo.SetValue(obj, newValue, null);
                    }
                }
            }
        }
    }
}
