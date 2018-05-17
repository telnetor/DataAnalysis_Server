using Dapper;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Core.Data.Entity.UnitTestEntity;
using DataAnalysis.Core.Data.IRepositories;
using DataAnalysis.Manipulation;
using DataAnalysis.Manipulation.Base;
using DataAnalysis.Manipulation.BuildSQLText;
using DataAnalysis.Manipulation.DapperExtension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Core.Data.Repository.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IDbConnection _connection;

        public IUnitOfWork UnitOfWork { get; }
        private readonly GenerateSql<TEntity> _generateSql;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _connection = unitOfWork.DbConnection;
            _generateSql = new GenerateSqlMssql<TEntity>();
            var dbProvider = unitOfWork.DbProviderConfig.ProviderFactoryString;
            switch (dbProvider)
            {
                case "MySql.Data.MySqlClient":
                    _generateSql = new GenerateSqlMysql<TEntity>();
                    break;
                case "System.Data.SqlClient":
                    _generateSql = new GenerateSqlMssql<TEntity>();
                    break;
                default:
                    throw new Exception("DBProvider 配置异常！");
            }
        }


        public void Dispose()
        {
            if (UnitOfWork != null)
                UnitOfWork.Dispose();
        }

        public int Add<TModel>(TModel entity, IDbTransaction transaction = null) where TModel : BaseEntity, new()
        {
            var addSql = _generateSql.GenerateInsertSqlTextAndParam();
            var args = new DynamicParameters(entity);
            if (addSql.Item2 != null)
                addSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value, p.DbType, p.Direction));

            string idtName = _generateSql.GetIdentityFields();//获得自增长列名
            int addResult = 0;
            if (!string.IsNullOrEmpty(idtName))
            {
                addResult = _connection.ExecuteScalar<int>(addSql.Item1, args, transaction);
                var dic = new Dictionary<string, object> { { idtName, addResult } };
                ReflectionHelper.SetPropertyValue(entity, dic);//设置自增长id的值
            }
            else
            {
                addResult = _connection.Execute(addSql.Item1, args, transaction);
            }
            _connection.Dispose();
            RecordSql(addSql.Item1);
            return addResult;
        }

        public long Count(Expression<Func<TEntity, bool>> filterExpression)
        {
            var modSql = _generateSql.GenerateCountSqlTextAndParam(filterExpression);

            var args = new DynamicParameters();
            if (modSql.Item2 != null)
                modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
            var modResult = _connection.ExecuteScalar<int>(modSql.Item1, args);
            _connection.Close();
            RecordSql(modSql);
            return modResult;
        }

        public object ExcuteScalar(string sql, IDbTransaction transaction = null)
        {
            var res = _connection.ExecuteScalar(sql, transaction);
            RecordSql(sql);
            _connection.Close();
            return res;
        }

        public int ExecuteCommand(string sql, dynamic parameters = null, IDbTransaction transaction = null)
        {
            var res = _connection.Execute(sql, parameters as object, transaction);
            var item = parameters as Dictionary<string, string>;
            _connection.Close();
            return res;
        }

        public IEnumerable<TModel> ExecuteQuery<TModel>(string sql, dynamic parameters = null, IDbTransaction transaction = null) where TModel : BaseEntity
        {
            IEnumerable<TModel> res = _connection.Query<TModel>(sql, parameters as object, transaction);
            _connection.Close();
            return res;
        }

        public int ExecuteStoredProcedure(string storedProcedureName, params object[] parameters)
        {
            var res = _connection.Execute(storedProcedureName, parameters);
            _connection.Close();
            return res;
        }

        public IEnumerable<TModel> ExecuteStoredProcedure<TModel>(string storedProcedureName, params object[] parameters) where TModel : BaseEntity, new()
        {
            return _connection.Query<TModel>(storedProcedureName, parameters.Length > 0 ? parameters[0] : null);
        }

        public bool Exists(Expression<Func<TEntity, bool>> filterExpression)
        {
            return Count(filterExpression) > 0;
        }

        public TEntity First(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>> orderByExpression = null)
        {
            var modSql = _generateSql.GenerateGetFristSqlText(filterExpression, orderByExpression);

            var args = new DynamicParameters();
            if (modSql.Item2 != null)
                modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));

            var mod = _connection.Query<TEntity>(modSql.Item1, args).FirstOrDefault();
            _connection.Close();
            RecordSql(modSql);
            return mod;
        }

        public TEntity Get(params object[] keyValues)
        {
            var modSql = _generateSql.GenerateGetByKeySqlText(keyValues);

            var args = new DynamicParameters();
            if (modSql.Item2 != null)
                modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
            var modResult = _connection.Query<TEntity>(modSql.Item1, args);
            _connection.Close();
            RecordSql(modSql);
            return modResult.SingleOrDefault();
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> filterExpression = null, Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>> orderByExpression = null, Expression<Func<TEntity, TEntity>> selectExpression = null)
        {
            IEnumerable<TEntity> modResult = null;
            var modSql = _generateSql.GenerateGetListSqlText(filterExpression, orderByExpression, selectExpression);
            var args = new DynamicParameters();
            if (modSql.Item2 != null)
                modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
            modResult = _connection.Query<TEntity>(modSql.Item1, args);
            _connection.Close();
            RecordSql(modSql);
            return modResult.ToList();
        }

        public int Modify(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> filterExpression, IDbTransaction transaction = null)
        {
            var modSql = _generateSql.GenerateUpdateSqlTextAndParam(updateExpression, filterExpression);

            var args = new DynamicParameters();
            if (modSql.Item2 != null)
                modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
            int modResult = _connection.Execute(modSql.Item1, args, transaction);
            _connection.Close();
            RecordSql(modSql);
            return modResult;
        }

        public int Remove(Expression<Func<TEntity, bool>> filterExpression, IDbTransaction transaction = null)
        {
            var delSql = _generateSql.GenerateDeleteSqlTextAndParam(filterExpression);

            var args = new DynamicParameters();
            if (delSql.Item2 != null)
                delSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));

            int delResult = _connection.Execute(delSql.Item1, args, transaction);
            _connection.Close();
            RecordSql(delSql);
            return delResult;
        }

        public TEntity Single(Expression<Func<TEntity, bool>> filterExpression)
        {
            if (Count(filterExpression) > 1 || Count(filterExpression) == 0) throw new Exception("The query does not return exactly one item.");
            return First(filterExpression);
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
