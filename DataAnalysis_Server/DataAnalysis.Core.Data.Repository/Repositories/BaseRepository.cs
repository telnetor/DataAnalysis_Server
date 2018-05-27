using Dapper;
using DataAnalysis.Component.Tools.Common;
using DataAnalysis.Component.Tools.Log;
using DataAnalysis.Core.Data.Entity;
using DataAnalysis.Core.Data.Enum;
using DataAnalysis.Core.Data.IRepositories;
using DataAnalysis.Manipulation.Base;
using DataAnalysis.Manipulation.BuildSQLText;
using DataAnalysis.Manipulation.DapperExtension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace DataAnalysis.Core.Data.Repository.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IDbConnection _connection;

        public IUnitOfWork UnitOfWork { get; }
        private readonly GenerateSql<TEntity> _generateSql;
        private readonly RecordInvokeSql<TEntity> _recordInvokeSql;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            _connection = unitOfWork.DbConnection;
            _generateSql = new GenerateSqlMssql<TEntity>();
            var dbProvider = unitOfWork.DbProviderConfig.ProviderFactoryString;
            _recordInvokeSql = new RecordInvokeSql<TEntity>();
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

        public ResponseMsg<int> Add<TModel>(TModel entity, IDbTransaction transaction = null) where TModel : BaseEntity, new()
        {
            ResponseMsg<int> responseMsg = new ResponseMsg<int>();
            Tuple<string, IDbDataParameter[]> addSql =null;
            DynamicParameters args = new DynamicParameters();
            string idtName = string.Empty;
            int addResult = 0;
            try
            {
                addSql = _generateSql.GenerateInsertSqlTextAndParam();
                args = new DynamicParameters(entity);
                if (addSql.Item2 != null)
                    addSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value, p.DbType, p.Direction));
                idtName = _generateSql.GetIdentityFields();//获得自增长列名
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
                if (addResult > 0)
                {
                    responseMsg = responseMsg.Ok(addResult);
                }
            }
            catch (Exception ex)
            {
                _recordInvokeSql.RecordSql<TModel>(entity, addSql.Item1, idtName);
                responseMsg = responseMsg.Error(ex);
                LogManage.Error.Debug(ex.Message);
            }
            finally
            {
                _connection.Dispose();
            }
        
            return responseMsg;
        }

        public ResponseMsg<int> Count(Expression<Func<TEntity, bool>> filterExpression)
        {
            ResponseMsg<int> responseMsg = new ResponseMsg<int>();
            Tuple<string, IDbDataParameter[]> modSql = null;
            var args = new DynamicParameters();
            try
            {
                modSql = _generateSql.GenerateCountSqlTextAndParam(filterExpression);
                if (modSql.Item2 != null)
                    modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
                var modResult = _connection.ExecuteScalar<int>(modSql.Item1, args);
                responseMsg=responseMsg.Ok(modResult);
            }
            catch (Exception ex)
            {
                responseMsg=responseMsg.Error(ex);
                _recordInvokeSql.RecordSql(modSql);
                LogManage.Error.Debug(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
            return responseMsg;
        }

        public ResponseMsg<object> ExcuteScalar(string sql, IDbTransaction transaction = null)
        {
            object res = new object();
            ResponseMsg<object> responseMsg = new ResponseMsg<object>();
            try
            {
                res = _connection.ExecuteScalar(sql, transaction);
                responseMsg = responseMsg.Ok(res);
            }
            catch (Exception ex)
            {
                responseMsg.Error(ex);
                _recordInvokeSql.RecordSql(sql);
                LogManage.Error.Debug(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
            return responseMsg;
        }

        public ResponseMsg<int> ExecuteCommand(string sql, dynamic parameters = null, IDbTransaction transaction = null)
        {
            ResponseMsg<int> responseMsg = new ResponseMsg<int>();
            int res = 0;
            try
            {
                res = _connection.Execute(sql, parameters as object, transaction);
                responseMsg = responseMsg.Ok(res);
            }
            catch (Exception ex)
            {
                responseMsg=responseMsg.Error(ex);
                _recordInvokeSql.RecordSql(sql);
                LogManage.Error.Debug(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
            return responseMsg;
        }

        public ResponseMsg<IEnumerable<TModel>> ExecuteQuery<TModel>(string sql, dynamic parameters = null, IDbTransaction transaction = null) where TModel : BaseEntity
        {
            ResponseMsg<IEnumerable<TModel>> responseMsg = new ResponseMsg<IEnumerable<TModel>>();
            IEnumerable<TModel> res = null;
            try
            {
                res = _connection.Query<TModel>(sql, parameters as object, transaction);
                responseMsg = responseMsg.Ok(res);
            }
            catch (Exception ex)
            {
                responseMsg=responseMsg.Error(ex);
                _recordInvokeSql.RecordSql(sql);
                LogManage.Error.Debug(ex.Message);
            }
            finally
            {
                _connection.Close();
            }
            return responseMsg;
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

        public ResponseMsg<bool> Exists(Expression<Func<TEntity, bool>> filterExpression)
        {
            ResponseMsg<int> responseMsg = Count(filterExpression);
            ResponseMsg<bool> response = new ResponseMsg<bool>();
            if (responseMsg.StatusCode == (int)StatusCodeEnum.Success)
            {
                response=response.Ok(responseMsg.Data > 0);
            }
            else
            {
                response=response.Error(new Exception(responseMsg.StatusMsg));
            }
            return response;
        }

        public ResponseMsg<TEntity> First(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>> orderByExpression = null)
        {
            ResponseMsg<TEntity> responseMsg = new ResponseMsg<TEntity>();
            Tuple<string, IDbDataParameter[]> modSql = null;
            DynamicParameters args = new DynamicParameters();
            try
            {
                modSql = _generateSql.GenerateGetFristSqlText(filterExpression, orderByExpression);
                args = new DynamicParameters();
                if (modSql.Item2 != null)
                    modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
                var mod = _connection.Query<TEntity>(modSql.Item1, args).FirstOrDefault();
                responseMsg=responseMsg.Ok(mod);
            }
            catch (Exception ex)
            {
                _connection.Close();
                responseMsg = responseMsg.Error(ex);
                _recordInvokeSql.RecordSql(modSql.Item1);
                LogManage.Error.Debug(ex);
            }

            return responseMsg;
        }

        public ResponseMsg<TEntity> Get(params object[] keyValues)
        {
            Tuple<string, IDbDataParameter[]> modSql = null;
            DynamicParameters args = new DynamicParameters();
            ResponseMsg<TEntity> responseMsg = new ResponseMsg<TEntity>();
            try
            {
                modSql = _generateSql.GenerateGetByKeySqlText(keyValues);
                args = new DynamicParameters();
                if (modSql.Item2 != null)
                    modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
                var modResult = _connection.Query<TEntity>(modSql.Item1, args);
                responseMsg=responseMsg.Ok(modResult.SingleOrDefault());
            }
            catch (Exception ex)
            {
                _connection.Close();
                responseMsg = responseMsg.Error(ex);
                _recordInvokeSql.RecordSql(modSql.Item1);
                LogManage.Error.Debug(ex);
            }
            finally
            {
                _connection.Close();
            }
            return responseMsg;
        }

        public ResponseMsg<List<TEntity>> GetList(Expression<Func<TEntity, bool>> filterExpression = null, Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>> orderByExpression = null, Expression<Func<TEntity, TEntity>> selectExpression = null)
        {
            ResponseMsg<List<TEntity>> responseMsg = new ResponseMsg<List<TEntity>>();
            IEnumerable<TEntity> modResult = null;
            Tuple<string, IDbDataParameter[]> modSql = null;
            DynamicParameters args = new DynamicParameters();
            try
            {
                modSql = _generateSql.GenerateGetListSqlText(filterExpression, orderByExpression, selectExpression);
                args = new DynamicParameters();
                if (modSql.Item2 != null)
                    modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
                modResult = _connection.Query<TEntity>(modSql.Item1, args);
                responseMsg=responseMsg.Ok(modResult.ToList());
            }
            catch (Exception ex)
            {
                _connection.Close();
                responseMsg = responseMsg.Error(ex);
                _recordInvokeSql.RecordSql(modSql.Item1);
                LogManage.Error.Debug(ex);
            }
            finally
            {
                _connection.Close();
            }
            return responseMsg;
        }

        public ResponseMsg<int> Modify(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> filterExpression, IDbTransaction transaction = null)
        {
            Tuple<string, IDbDataParameter[]> modSql = null;
            DynamicParameters args = new DynamicParameters();
            ResponseMsg<int> responseMsg = new ResponseMsg<int>();
            try
            {
                modSql = _generateSql.GenerateUpdateSqlTextAndParam(updateExpression, filterExpression);
                args = new DynamicParameters();
                if (modSql.Item2 != null)
                    modSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
                int modResult = _connection.Execute(modSql.Item1, args, transaction);
                responseMsg=responseMsg.Ok(modResult);
            }
            catch (Exception ex)
            {
                LogManage.Error.Debug(ex);
                _recordInvokeSql.RecordSql(modSql.Item1);
                responseMsg=responseMsg.Error(ex);
            }
            finally
            {
                _connection.Close();
            }
          
            return responseMsg;
        }

        public ResponseMsg<int> Remove(Expression<Func<TEntity, bool>> filterExpression, IDbTransaction transaction = null)
        {
            Tuple<string, IDbDataParameter[]> delSql = null;
            DynamicParameters args = new DynamicParameters();
            ResponseMsg<int> responseMsg = new ResponseMsg<int>();
            int delResult = 0;
            try
            {
                delSql = _generateSql.GenerateDeleteSqlTextAndParam(filterExpression);
                args = new DynamicParameters();
                if (delSql.Item2 != null)
                    delSql.Item2.ForAll(p => args.Add(p.ParameterName, p.Value));
                delResult = _connection.Execute(delSql.Item1, args, transaction);
                responseMsg=responseMsg.Ok(delResult);
            }
            catch (Exception ex)
            {
                LogManage.Error.Debug(ex);
                _recordInvokeSql.RecordSql(delSql.Item1);
                responseMsg=responseMsg.Error(ex);
            }
            finally
            {
                _connection.Close();
            }
            return responseMsg;
        }

        public ResponseMsg<TEntity> Single(Expression<Func<TEntity, bool>> filterExpression)
        {
            ResponseMsg<int> response = Count(filterExpression);
            if (response.StatusCode == (int)StatusCodeEnum.Success)
            {
                if (response.Data > 1 || response.Data == 0) throw new Exception("The query does not return exactly one item.");
                return First(filterExpression);
            }
            ResponseMsg<TEntity> responseMsg = new ResponseMsg<TEntity>();
            return responseMsg.Error(new Exception(response.StatusMsg));
        }
    }
}
