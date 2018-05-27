using DataAnalysis.Component.Tools.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAnalysis.Manipulation.BuildSQLText
{
    public class RecordInvokeSql<T>
    {
        public async void RecordSql(string sqlText, dynamic dynamic)
        {
            await Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(sqlText)) throw new Exception("解析Sql不能为空");
                var res = dynamic as object;
                if (res != null)
                {
                    string tempPrams = res.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
                    var listPrams = tempPrams.Split(new string[] { "," }, StringSplitOptions.None).ToList();
                    var sqlTemp = sqlText;
                    listPrams.ForEach(p =>
                    {
                        var keyValueArray = p.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (keyValueArray.Length == 2)
                        {
                            string key = keyValueArray[0];
                            string value = keyValueArray[1];
                            string regex = $@"{key.Trim()}\s*=\s*@{key.Trim()}";
                            sqlTemp = Regex.Replace(sqlTemp, regex, $"{key}={value}");
                        }
                    });
                    LogManage.Sql.Info(sqlTemp);
                }
            });
        }

        public async void RecordSql<T>(T t, string sqlText, string idName = "")
        {
            await Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(sqlText)) throw new Exception("解析Sql不能为空");
                PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (properties.Length > 0)
                {
                    string sql = sqlText;
                    foreach (PropertyInfo item in properties)
                    {
                        if (item.Name.Equals(idName))
                        {
                            continue;
                        }
                        string name = item.Name;
                        object value = item.GetValue(t, null);
                        if (item.PropertyType.Name.Equals("System"))
                        {
                            sql = sql.Replace($"@{name}", $"'{value.ToString()}'");
                        }
                        else if (item.PropertyType.Name.Equals("Int32"))
                        {
                            sql = sql.Replace($"@{name}", $"{value.ToString()}");
                        }
                        else
                        {
                            sql = sql.Replace($"@{name}", $"'{value.ToString()}'");
                        }
                    }
                    LogManage.Sql.Info(sql);
                }
            });
        }
        public async void RecordSql(string sqlText)
        {
            await Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(sqlText)) throw new Exception("解析Sql不能为空");
                LogManage.Sql.Info(sqlText);
            });
        }
        public async void RecordSql(Tuple<string, IDbDataParameter[]> tuple)
        {
            await Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrWhiteSpace(tuple.Item1)) throw new Exception("解析Sql不能为空");
                string sqlText = tuple.Item1;
                IDbDataParameter[] dbParams = tuple.Item2;
                if (dbParams != null && dbParams.Count() > 0)
                {
                    var sqlTemp = sqlText;
                    dbParams.ToList().ForEach(p =>
                    {
                        string parameterName = p.ParameterName;
                        if (p.DbType == DbType.String)
                        {
                            sqlTemp = sqlTemp.Replace(parameterName, $"'{p.Value}'");
                        }
                        else if (p.DbType == DbType.Int32)
                        {
                            sqlTemp = sqlTemp.Replace(parameterName, $"{p.Value}");
                        }
                    });
                    LogManage.Sql.Info(sqlTemp);
                }
                else
                {
                    LogManage.Sql.Info(sqlText);
                }
            });
        }
    }
}
