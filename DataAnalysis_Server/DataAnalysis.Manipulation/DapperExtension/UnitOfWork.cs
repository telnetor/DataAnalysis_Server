using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis.Manipulation.DapperExtension
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbTransaction _transaction;
        private readonly IDbConnection _connection;

        public UnitOfWork(IDbProviderConfig dbProviderConfig)
        {
            this._connection = Database.DbService(dbProviderConfig);
            DbProviderConfig = dbProviderConfig;
        }

        public IDbProviderConfig DbProviderConfig { get; }

        public IDbConnection DbConnection => _connection;

        public int Commit()
        {
            if (IsInTransaction)
            {
                try
                {
                    _transaction.Commit();
                }
                catch
                {
                    _transaction.Rollback();
                    throw;
                }
                finally
                {
                    ReleaseCurrentTransaction();
                }

                return 1;
            }

            return 0;
        }

        public bool IsInTransaction
        {
            get { return _transaction != null; }
        }

        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);

        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_transaction != null)
            {
                throw new ApplicationException("Cannot begin a new transaction while an existing transaction is still running. " +
                                                "Please commit or rollback the existing transaction before starting a new one.");
            }
            OpenConnection();
            _transaction = _connection.BeginTransaction(isolationLevel); ;
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }

        public void CommitTransaction()
        {
            if (_transaction == null)
            {
                throw new ApplicationException("Cannot roll back a transaction while there is no transaction running.");
            }

            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                ReleaseCurrentTransaction();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes off the managed and unmanaged resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_connection != null && _connection.State == ConnectionState.Open)
                _connection.Dispose();

            ReleaseCurrentTransaction();

        }

        private void OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

        private void ReleaseCurrentTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
