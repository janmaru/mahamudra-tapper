using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Mahamudra.Tapper.Interfaces
{
    public interface IPersistence
    {
        Task<GridReader> SelectMultipleAsync<T>(IDbConnection connection, string sqlCommand, object parameters, IDbTransaction transaction = null, CommandType type = CommandType.Text, int? commandTimeout = null); 
        Task<T> ExecuteAsync<T>(IDbConnection connection, string sqlCommand, object parameters, IDbTransaction transaction = null, CommandType type = CommandType.Text, int? commandTimeout = null);
        Task<int> ExecuteBatchAsync(IDbConnection connection, string sqlCommand, IEnumerable<object> parameters, IDbTransaction transaction = null, CommandType type = CommandType.Text, int? commandTimeout = null);
        Task<IEnumerable<T>> SelectAsync<T>(IDbConnection connection, string sqlCommand, object parameters, IDbTransaction transaction = null, CommandType type = CommandType.Text, bool buffered = true, int? commandTimeout = null);
        Task<IEnumerable<T>> SelectAsync<T, S>(IDbConnection connection, string sqlQuery, Func<T, S, T> map, string splitOn, object parameters, IDbTransaction transaction = null, CommandType type = CommandType.Text, bool buffered = true, int? commandTimeout = null);
        Task<T> SingleAsync<T>(IDbConnection connection, string sqlCommand, object parameters, IDbTransaction transaction = null, CommandType type = CommandType.Text, int? commandTimeout = null);
    }
}