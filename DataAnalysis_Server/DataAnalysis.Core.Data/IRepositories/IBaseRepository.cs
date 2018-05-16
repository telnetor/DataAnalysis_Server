
using DataAnalysis.Manipulation.Base;
using DataAnalysis.Manipulation.DapperExtension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Core.Data.IRepositories
{
    public interface IBaseRepository<TEntity> : IDisposable where TEntity : BaseEntity
    {
        IUnitOfWork UnitOfWork { get; }
        /// <summary>
        /// 添加
        /// </summary>
        int Add<TModel>(TModel entity, IDbTransaction transaction = null) where TModel : BaseEntity, new();

        /// <summary>
        /// 删除
        /// </summary>
        int Remove(Expression<Func<TEntity, bool>> filterExpression, IDbTransaction transaction = null);

        /// <summary>
        /// 修改
        /// </summary>
        int Modify(Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> filterExpression, IDbTransaction transaction = null);

        /// <summary>
        /// 查询根据主键
        /// </summary>
        TEntity Get(params object[] keyValues);

        /// <summary>
        /// 查询列表
        /// </summary>
        List<TEntity> GetList(
            Expression<Func<TEntity, bool>> filterExpression = null,
            Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>> orderByExpression = null,
            Expression<Func<TEntity, TEntity>> selectExpression = null);

        /// <summary>
        /// 查询单行记录
        /// </summary>
        TEntity Single(Expression<Func<TEntity, bool>> filterExpression);

        /// <summary>
        /// 查询第一行记录
        /// </summary>
        TEntity First(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<IQueryable<TEntity>, IQueryable<TEntity>>> orderByExpression = null);

        /// <summary>
        /// 查询记录总数
        /// </summary>
        long Count(Expression<Func<TEntity, bool>> filterExpression);

        /// <summary>
        /// 查询记录是否存在
        /// </summary>
        bool Exists(Expression<Func<TEntity, bool>> filterExpression);

        /// <summary>
        /// 执行SQL语句，返回受影响行数
        /// </summary>
        int ExecuteCommand(string sql, dynamic parameters = null, IDbTransaction transaction = null);

        /// <summary>
        /// 执行SQL查询语句，返回列表
        /// </summary>
        IEnumerable<TModel> ExecuteQuery<TModel>(string sql, dynamic parameters = null, IDbTransaction transaction = null) where TModel : BaseEntity;

        /// <summary>
        /// 执行存储过程，返回受影响行数
        /// </summary>
        int ExecuteStoredProcedure(string storedProcedureName, params object[] parameters);

        /// <summary>
        /// 执行存储过程，返回列表
        /// </summary>
        IEnumerable<TModel> ExecuteStoredProcedure<TModel>(string storedProcedureName, params object[] parameters) where TModel : BaseEntity, new();

        /// <summary>
        /// 执行无参SQL
        /// </summary>
        object ExcuteScalar(string sql, IDbTransaction transaction = null);
    }
}
