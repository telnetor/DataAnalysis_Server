
using DataAnalysis.Manipulation.Base;
using DataAnalysis.Manipulation.ExpressionResolve;
using DataAnalysis.Manipulation.ExpressionResolve.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataAnalysis.Manipulation.BuildSQLText
{
    public class GenerateSqlMysql<T> : GenerateSql<T> where T : BaseEntity
    {

        public override Tuple<string, IDbDataParameter[]> GenerateTopCountSqlText(int topCount, Expression<Func<T, bool>> filterExpression, Expression<Func<IQueryable<T>, IQueryable<T>>> orderByExpression)
        {
            ResolveExpression resolve = ResolveToSql(filterExpression);

            StringBuilder strSql = new StringBuilder();

            string tableName = GetTableName();

            strSql.AppendFormat("Select * from {0}", tableName);
            strSql.AppendFormat(" Where {0}", resolve.SqlWhere);

            if (orderByExpression != null)
            {
                var orderby = AiExpressionWriterSql.BizWhereWriteToString(orderByExpression, AiExpSqlType.aiOrder);
                strSql.AppendFormat(" Order by {0} LIMIT {1},{2};", orderby, 0, topCount);
            }
            else
            {
                strSql.AppendFormat(" LIMIT {0},{1};", 0, topCount);
            }

            var sqlText = strSql.ToString();

            return new Tuple<string, IDbDataParameter[]>(sqlText, resolve.Paras);
        }

      

    }
}
