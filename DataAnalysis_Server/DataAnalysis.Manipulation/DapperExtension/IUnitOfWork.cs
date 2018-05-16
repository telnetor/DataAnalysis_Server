using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Manipulation.DapperExtension
{
    public interface IUnitOfWork : IDisposable
    {
        IDbProviderConfig DbProviderConfig { get; }

        IDbConnection DbConnection { get; }

        /// <summary>
        /// Commit all changes made in a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,  
        /// then an exception is thrown
        ///</remarks>
        int Commit();

        ///// <summary>
        ///// Commit all changes made in  a container.
        ///// </summary>
        /////<remarks>
        ///// If the entity have fixed properties and any optimistic concurrency problem exists,
        ///// then 'client changes' are refreshed - Client wins
        /////</remarks>
        //void CommitAndRefreshChanges();


        ///// <summary>
        ///// Rollback tracked changes. See references of UnitOfWork pattern
        ///// </summary>
        //void RollbackChanges();

        bool IsInTransaction { get; }

        void BeginTransaction();

        void BeginTransaction(IsolationLevel isolationLevel);

        void RollbackTransaction();

        void CommitTransaction();
    }
}
