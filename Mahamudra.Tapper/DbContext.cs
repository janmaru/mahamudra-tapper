using Mahamudra.Tapper.Interfaces;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Mahamudra.Tapper
{
    public class DbContext : IDbContext
    {
        private bool _disposed;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private string _schema;
        public DbContext(IDbConnection connection,
                      ITransaction transaction = null,
                      string schema = null)
        {
            _schema = schema;
            _connection = connection;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            if (transaction != null)
                _transaction = _connection.BeginTransaction(transaction.IsolationLevel);
        }

        public string GetSchema() => _schema;

        public Task<T> Query<T>(IQuery<T> query, CancellationToken ct = default)
            => query.Select(_connection, _transaction, ct, _schema);
 
        public Task<T> Execute<T>(ICommand<T> command, CancellationToken ct = default) 
            => command.Execute(_connection, _transaction, ct, _schema); 

        public void Commit()
            => _transaction?.Commit();

        public void Rollback()
            => _transaction?.Rollback();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DbContext()
            => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _transaction?.Dispose();
                if (_connection != null &&
                    _connection.State == ConnectionState.Open)
                    _connection.Close();
                _connection?.Dispose();
            }

            _transaction = null;
            _connection = null;

            _disposed = true;
        }
    }
}