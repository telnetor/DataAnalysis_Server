using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Manipulation.Base;
using DataAnalysis.Manipulation.ExpressionResolve;
using DataAnalysis.Manipulation.ExpressionResolve.Base;
using MySql.Data.MySqlClient;

namespace DataAnalysis.Manipulation.BuildSQLText
{
    public abstract class GenerateSql<T> where T : BaseEntity
    {
        #region 构造方法

        protected GenerateSql(DataBaseType dbType = DataBaseType.SqlServer, bool enablePluralization = true)
        {

            _dataBaseType = dbType;
            _entityAnaly = new EntityAnalyReflect(enablePluralization);
        }

        #endregion

        #region 变量

        readonly DataBaseType _dataBaseType;
        readonly EntityAnaly _entityAnaly;
        //readonly bool EnablePluralization;
        //readonly ResolveExpression _resolveExpression;

        #endregion

        /// <summary>
        /// 删除指定的忽略字段
        /// </summary>
        /// <param name="sourceFields">要删除的字段源</param>
        /// <param name="removeFields">指定要删除的字段</param>
        /// <returns>返回删除后的字段列表</returns>
        protected internal string[] RemoveFields(string[] sourceFields, params string[] removeFields)
        {
            if (removeFields == null || removeFields.Length == 0) return sourceFields;
            return (from t1 in sourceFields let ignore = removeFields.Any(t => t.Equals(t1, StringComparison.OrdinalIgnoreCase)) where ignore == false select t1).ToArray();
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        protected internal string GetTableName()
        {
            return _entityAnaly.GetTableName<T>();
        }
        /// <summary>
        /// 获取所有字段
        /// </summary>
        protected internal string[] GetAllFields()
        {
            Dictionary<string, FieldAttribute> modelAttr = _entityAnaly.GetEntityAttribute<T>();
            return modelAttr.Keys.Select(item => modelAttr[item].ColumnName).ToArray();
        }
        /// <summary>
        /// 获取Insert语句的字段,会去除自增长列名
        /// </summary>
        protected string[] GetInsertFields()
        {
            Dictionary<string, FieldAttribute> modelAttr = _entityAnaly.GetEntityAttribute<T>();
            List<string> fl = (from item in modelAttr.Keys where !modelAttr[item].IsIdentity select modelAttr[item].ColumnName).ToList();
            if (fl.Count == 0)
            {
                string errMsg = string.Format("解析{0}类的字段失败", typeof(T));
                throw new Exception(errMsg);
            }
            return fl.ToArray();
        }
        /// <summary>
        /// 获取自增长列名
        /// </summary>
        public string GetIdentityFields()
        {
            Dictionary<string, FieldAttribute> modelAttr = _entityAnaly.GetEntityAttribute<T>();
            string fl = (from item in modelAttr.Keys where modelAttr[item].IsPrimaryKey && modelAttr[item].IsIdentity select modelAttr[item].ColumnName).FirstOrDefault();
            return fl;
        }
        /// <summary>
        /// 获得数据库用的字段格式,[Id],[Name],[Age]
        /// </summary>        
        protected string GetSelectFieldStr(string[] fieldArr)
        {
            if (fieldArr == null || fieldArr.Length == 0) return null;
            StringBuilder fdStr = new StringBuilder();
            for (int i = 0; i < fieldArr.Length - 1; i++)
            {
                //fdStr.AppendFormat("[{0}],", fieldArr[i]);
                //2013.11.10 去掉查询时字段的"[]"(中括号),是因为SQLite冲突,
                //如果需要sql server 中加上[] 那么可在SQLite中重写该方法
                fdStr.AppendFormat("{0},", fieldArr[i]);
            }
            fdStr.AppendFormat("{0}", fieldArr[fieldArr.Length - 1]);
            return fdStr.ToString();
        }
        /// <summary>
        /// 获得数据库用的字段参数化格式,@Id,@Name,@Age
        /// </summary>        
        protected string GetSelectFieldParam(string[] fieldArr)
        {
            if (fieldArr == null || fieldArr.Length == 0) return null;
            StringBuilder fdStr = new StringBuilder();
            for (int i = 0; i < fieldArr.Length - 1; i++)
            {
                fdStr.AppendFormat("@{0},", fieldArr[i]);
            }
            fdStr.AppendFormat("@{0}", fieldArr[fieldArr.Length - 1]);
            return fdStr.ToString();
        }
        /// <summary>
        /// 获得主键字段名
        /// </summary>
        protected internal List<string> GetPrimaryKeyName()
        {
            Dictionary<string, FieldAttribute> modelAttr = _entityAnaly.GetEntityAttribute<T>();

            var list =
                modelAttr.Keys.Where(k => modelAttr[k].IsPrimaryKey).Select(k => modelAttr[k].ColumnName).ToList();
            //foreach (string item in modelAttr.Keys)
            //{
            //    if (modelAttr[item].IsPrimaryKey)
            //    {
            //        return modelAttr[item].ColumnName;
            //    }
            //}
            if (list == null || list.Count == 0)
                throw new Exception(string.Format("【{0}】表不存主键", _entityAnaly.GetTableName<T>()));

            return list;
        }
        /// <summary>
        /// 获得主键字段名
        /// </summary>
        public SqlDbType GetFieldType(T entity, string fieldName)
        {
            var entityType = typeof(T);

            Type fieldType = entityType.GetProperties().Single(p => p.Name == fieldName).PropertyType;

            var sqlDbType = ConvertType(fieldType);

            return sqlDbType;
        }

        private static SqlDbType ConvertType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean:
                    return SqlDbType.Bit;
                case TypeCode.Byte:
                    return SqlDbType.TinyInt;
                case TypeCode.DateTime:
                    return SqlDbType.DateTime;
                case TypeCode.Decimal:
                    return SqlDbType.Decimal;
                case TypeCode.Double:
                    return SqlDbType.Float;
                case TypeCode.Int16:
                    return SqlDbType.SmallInt;
                case TypeCode.Int32:
                    return SqlDbType.Int;
                case TypeCode.Int64:
                    return SqlDbType.BigInt;
                case TypeCode.SByte:
                    return SqlDbType.TinyInt;
                case TypeCode.Single:
                    return SqlDbType.Real;
                case TypeCode.String:
                    return SqlDbType.NVarChar;
                case TypeCode.UInt16:
                    return SqlDbType.SmallInt;
                case TypeCode.UInt32:
                    return SqlDbType.Int;
                case TypeCode.UInt64:
                    return SqlDbType.BigInt;
                default:
                    if (t == typeof(byte[]))
                    {
                        return SqlDbType.Binary;
                    }
                    return SqlDbType.Variant;
            }
        }

        private IDbDataParameter GetDbDataParameter()
        {
            switch (_dataBaseType)
            {
                case DataBaseType.MySql:
                    return new MySqlParameter();

                default:
                    return new SqlParameter();

            }
        }

        protected string ResolveSelectExpress<TModel>(Expression<Func<T, TModel>> selectExpression) where TModel : BaseEntity
        {
            #region

            StringBuilder strSql = new StringBuilder();

            var memberInitExpression = selectExpression.Body as MemberInitExpression;
            if (memberInitExpression == null)
                throw new ArgumentException("The select expression must be of type MemberInitExpression.", "selectExpression");


            bool wroteSet = false;
            foreach (MemberBinding binding in memberInitExpression.Bindings)
            {
                if (wroteSet)
                    strSql.AppendLine(", ");

                string propertyName = binding.Member.Name;



                var memberAssignment = binding as MemberAssignment;
                if (memberAssignment == null)
                    throw new ArgumentException("The select expression MemberBinding must only by type MemberAssignment.", "selectExpression");

                Expression memberExpression = memberAssignment.Expression;


                object value;

                if (memberExpression.NodeType == ExpressionType.Constant)
                {
                    var constantExpression = memberExpression as ConstantExpression;
                    if (constantExpression == null)
                        throw new ArgumentException(
                            "The MemberAssignment expression is not a ConstantExpression.", "selectExpression");

                    value = constantExpression.Value;
                }
                else
                {
                    var right = (MemberExpression)memberExpression;

                    value = right.Member.Name;
                }

                strSql.AppendFormat("{0} as {1}", value, propertyName);

                wroteSet = true;
            }

            return strSql.ToString();

            #endregion
        }

        protected string ResolveUpdateExpress(Expression<Func<T, T>> updateExpression, out List<IDbDataParameter> sqlParams)
        {
            #region

            sqlParams = new List<IDbDataParameter>();
            StringBuilder strSql = new StringBuilder();

            var memberInitExpression = updateExpression.Body as MemberInitExpression;
            if (memberInitExpression == null)
                throw new ArgumentException("The update expression must be of type MemberInitExpression.", "updateExpression");

            int nameCount = 0;
            bool wroteSet = false;
            foreach (MemberBinding binding in memberInitExpression.Bindings)
            {
                string propertyName = binding.Member.Name;

                var memberAssignment = binding as MemberAssignment;
                if (memberAssignment == null)
                    throw new ArgumentException("The update expression MemberBinding must only by type MemberAssignment.", "updateExpression");

                Expression memberExpression = memberAssignment.Expression;


                object value;

                if (memberExpression.NodeType == ExpressionType.Constant)
                {
                    var constantExpression = memberExpression as ConstantExpression;
                    if (constantExpression == null)
                        throw new ArgumentException(
                            "The MemberAssignment expression is not a ConstantExpression.", "updateExpression");

                    value = constantExpression.Value;
                }
                else
                {
                    LambdaExpression lambda = Expression.Lambda(memberExpression, null);
                    value = lambda.Compile().DynamicInvoke();
                }
                if (value != null)
                {
                    if (value is Int32)
                    {
                        if (Convert.ToInt32(value) == 0)
                        {
                            continue;
                        }
                    }
                    else if (value is Int64)
                    {
                        if (Convert.ToInt64(value) == 0)
                        {
                            continue;
                        }
                    }
                    else if (value is DateTime)
                    {
                        if ((DateTime)value == DateTime.MinValue)
                        {
                            continue;
                        }
                    }
                    string parameterName = string.Format("@u__{0}__{1}", propertyName, nameCount++);//string.Format("@{0}", propertyName);

                    var parameter = GetDbDataParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = value;


                    sqlParams.Add(parameter);
                    if (wroteSet)
                        strSql.AppendLine(", ");
                    strSql.AppendFormat("{0} = {1}", propertyName, parameterName);
                    wroteSet = true;
                }
                else
                {
                    //strSql.AppendFormat("{0} = NULL", propertyName);
                    //wroteSet = false;
                }
            }

            return strSql.ToString();


            #endregion
        }



        /// <summary>
        /// 反射获取字段名与对应的字段值,Key:数据库字段名(非类的属性名),解决类中属性名与数据字段必须一致的方法
        /// </summary>
        /// <param name="model">实体对象</param>        
        protected Dictionary<string, object> GetModelValue(T model)
        {
            Dictionary<string, FieldAttribute> modelAttr = _entityAnaly.GetEntityAttribute<T>();
            Dictionary<string, object> r = new Dictionary<string, object>();
            foreach (string modelAttrName in modelAttr.Keys)
            {
                PropertyInfo proInfo = ReflectionHelper.GetPropertyInfo<T>(modelAttrName);
                string fieldName = modelAttr[modelAttrName].ColumnName;
                object attrValue = proInfo.GetValue(model, null);
                Type type = proInfo.PropertyType;
                //SetDefaultValue(type, ref attrValue);
                r.Add(modelAttr[modelAttrName].ColumnName, attrValue);
            }
            return r;
            ////类的属性名与之属性值
            //var obj = ReflectionHelper.GetPropertyNameAndValue(model);

            //foreach (string item in modelAttr.Keys)
            //{

            //    //TODO:根据Model的属性名设置Model的默认值,在此加入代码
            //    r.Add(modelAttr[item].ColumnName, obj[item]);
            //}
            //return r;
        }
        /// <summary>
        /// 判断sqlWhere是否包含where,如果不包含则开始位置插入where,
        /// 如果sqlwhere为Null或者为空字符串,则返回false,否则返回true;
        /// </summary>
        protected internal bool CheckSqlWhere(ref string sqlWhere)
        {
            if (string.IsNullOrEmpty(sqlWhere)) return false;
            if (sqlWhere.Trim().Length == 0) return false;
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                if (!sqlWhere.Contains("where"))
                {
                    sqlWhere = sqlWhere.Insert(0, " where ");
                }
            }
            return true;
        }

        protected internal ResolveExpression ResolveToSql(Expression<Func<T, bool>> filterExpression)
        {


            ResolveExpression resolve = new ResolveExpression();
            resolve.ResolveToWhere(filterExpression);
            return resolve;
        }

        /// <summary>
        /// 生成insert语句的SQL文本和参数信息
        /// </summary>
        public virtual Tuple<string, IDbDataParameter[]> GenerateInsertSqlTextAndParam()
        {
            StringBuilder strSql = new StringBuilder();
            //UPDATE:应更改Exception为自定义
            string[] addFields = GetInsertFields();
            string tableName = GetTableName();
            string addFieldStr = GetSelectFieldStr(addFields);
            string addFieldParam = GetSelectFieldParam(addFields);
            strSql.AppendFormat("Insert into {0}({1})  Values ({2});", tableName, addFieldStr, addFieldParam);
            List<IDbDataParameter> sqlParams = new List<IDbDataParameter>();


            string identityField = GetIdentityFields();

            if (!string.IsNullOrEmpty(identityField))
            {
                string parameterName = "@i_idt_id";//string.Format("@{0}", propertyName);
                var parameter = GetDbDataParameter();
                parameter.ParameterName = parameterName;
                parameter.DbType = DbType.Int32;
                parameter.Direction = ParameterDirection.Output;
                sqlParams.Add(parameter);

                strSql.AppendFormat("SELECT @@IDENTITY;", parameterName);
            }

            string sqlText = strSql.ToString();
            return new Tuple<string, IDbDataParameter[]>(sqlText, sqlParams.ToArray());

        }

        /// <summary>
        /// 生成Update语句的sql文本和参数信息
        /// </summary>
        public virtual Tuple<string, IDbDataParameter[]> GenerateUpdateSqlTextAndParam(Expression<Func<T, T>> updateExpression, Expression<Func<T, bool>> filterExpression)
        {

            StringBuilder strSql = new StringBuilder();
            List<IDbDataParameter> sqlParams;

            string tableName = GetTableName();

            string updateField = ResolveUpdateExpress(updateExpression, out sqlParams);

            strSql.AppendFormat("Update {0} Set {1}", tableName, updateField);



            ResolveExpression resolve = ResolveToSql(filterExpression);
            strSql.AppendFormat(" Where {0}", resolve.SqlWhere);
            sqlParams.AddRange(resolve.Paras.ToList());

            var sqlText = strSql.ToString();

            return new Tuple<string, IDbDataParameter[]>(sqlText, sqlParams.ToArray());
        }

        /// <summary>
        /// 生成删除的sql文本和参数
        /// </summary>
        public virtual Tuple<string, IDbDataParameter[]> GenerateDeleteSqlTextAndParam(Expression<Func<T, bool>> filterExpression)
        {
            ResolveExpression resolve = ResolveToSql(filterExpression);

            StringBuilder strSql = new StringBuilder();

            string tableName = GetTableName();

            strSql.AppendFormat("Delete from {0}", tableName);
            strSql.AppendFormat(" Where {0};", resolve.SqlWhere);

            var sqlText = strSql.ToString();

            return new Tuple<string, IDbDataParameter[]>(sqlText, resolve.Paras);
        }

        /// <summary>
        /// 判断记录是否存在的sql问和参数
        /// </summary>
        public virtual void GenerateIsExistSqlText(object pkeyValue, string fieldName, object fieldValue, string sqlWhere, ref Dictionary<string, object> dbParams, out string sqlText)
        {
            StringBuilder strSql = new StringBuilder();
            string tableName = GetTableName();
            strSql.AppendFormat("Select Count(1) From {0} ", tableName);
            bool isUseWhere = false;
            if (CheckSqlWhere(ref sqlWhere))
            {
                strSql.Append(sqlWhere);
                //strSql.Append(" and ");
                isUseWhere = true;
            }
            else
            {
                strSql.AppendLine(" where ");
                if (dbParams == null) dbParams = new Dictionary<string, object>();
            }
            if (pkeyValue != null)
            {
                if (isUseWhere)
                {
                    strSql.Append(" and ");
                }
                List<string> pkNames = GetPrimaryKeyName();
                foreach (string pkName in pkNames)
                {
                    //不包含该主键值的其它数据是否存在
                    string pkParamName = string.Format("@{0}_z", pkName);
                    strSql.AppendFormat("{0}<>{1} and", pkName, pkParamName);
                    dbParams.Add(pkParamName, pkeyValue);
                }

            }
            if (!string.IsNullOrEmpty(fieldName))
            {
                if (isUseWhere)
                {
                    strSql.Append(" and ");
                }
                string fieldParamName = string.Format("@{0}_z", fieldName);
                strSql.AppendFormat(" {0}={1}", fieldName, fieldParamName);
                dbParams.Add(fieldParamName, fieldValue);
            }
            sqlText = strSql.ToString();
        }

        /// <summary>
        /// 生成统计记录数的sql文本和参数
        /// </summary>
        public virtual Tuple<string, IDbDataParameter[]> GenerateCountSqlTextAndParam(Expression<Func<T, bool>> filterExpression)
        {

            var strSql = new StringBuilder();
            string tableName = _entityAnaly.GetTableName<T>();
            strSql.AppendFormat("Select Count(1) as CountRecord From {0} ", tableName);

            ResolveExpression resolve = ResolveToSql(filterExpression);

            strSql.AppendFormat(" Where {0};", resolve.SqlWhere);

            var sqlText = strSql.ToString();

            return new Tuple<string, IDbDataParameter[]>(sqlText, resolve.Paras);
        }

        /// <summary>
        /// 生成获取列表sql
        /// </summary>
        public virtual Tuple<string, IDbDataParameter[]> GenerateGetListSqlText<TModel>(
            Expression<Func<T, bool>> filterExpression = null,
            Expression<Func<IQueryable<T>, IQueryable<T>>> orderByExpression = null,
            Expression<Func<T, TModel>> selectExpression = null) where TModel : BaseEntity
        {

            StringBuilder strSql = new StringBuilder();

            string tableName = GetTableName();

            string selectField = selectExpression == null ? "*" : ResolveSelectExpress(selectExpression);

            strSql.AppendFormat("Select {1} from {0}", tableName, selectField);

            IDbDataParameter[] dbParams = null;
            if (filterExpression != null)
            {
                ResolveExpression resolve = ResolveToSql(filterExpression);
                strSql.AppendFormat(" Where {0}", resolve.SqlWhere);
                dbParams = resolve.Paras;
            }

            if (orderByExpression != null)
            {
                var orderby = ExpressionResolve.Base.AiExpressionWriterSql.BizWhereWriteToString(orderByExpression, AiExpSqlType.aiOrder);
                strSql.AppendFormat(" Order by {0};", orderby);
            }


            var sqlText = strSql.ToString();

            return new Tuple<string, IDbDataParameter[]>(sqlText, dbParams);
        }

        /// <summary>
        /// 生成获取列表sql
        /// </summary>
        public virtual Tuple<string, IDbDataParameter[]> GenerateGetByKeySqlText(params object[] values)
        {

            StringBuilder strSql = new StringBuilder();
            List<IDbDataParameter> dbParams = new List<IDbDataParameter>();

            string tableName = GetTableName();

            strSql.AppendFormat("Select * from {0}", tableName);
            strSql.AppendFormat(" Where");

            List<string> pkNames = GetPrimaryKeyName();

            if (values.Length != pkNames.Count) throw new ArgumentException("查询的主键数量与实体主键不一致");

            for (int i = 0; i < pkNames.Count; i++)
            {
                string pkName = pkNames[i];

                var pkValue = values[i];

                string pkParamName = string.Format("@{0}_z", pkName);

                strSql.AppendFormat(i == 0 ? " {0}={1}" : " AND {0}={1}", pkName, pkParamName);

                var param = GetDbDataParameter();
                param.ParameterName = pkParamName;
                param.Value = pkValue;
                dbParams.Add(param);
            }

            var sqlText = strSql.ToString();

            return new Tuple<string, IDbDataParameter[]>(sqlText, dbParams.ToArray());
        }

        /// <summary>
        /// 生成获取列表sql
        /// </summary>
        public virtual Tuple<string, IDbDataParameter[]> GenerateGetFristSqlText(Expression<Func<T, bool>> filterExpression, Expression<Func<IQueryable<T>, IQueryable<T>>> orderByExpression)
        {
            return GenerateTopCountSqlText(1, filterExpression, orderByExpression);
        }

        /// <summary>
        /// 生成前几行的数据和sql文本和参数
        /// </summary>
        public abstract Tuple<string, IDbDataParameter[]> GenerateTopCountSqlText(int topCount, Expression<Func<T, bool>> filterExpression, Expression<Func<IQueryable<T>, IQueryable<T>>> orderByExpression);


    }
}
