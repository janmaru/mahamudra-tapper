using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using Mahamudra.Tapper.Interfaces;
using static Dapper.SqlMapper;

namespace Mahamudra.Tapper
{
    public abstract class DapperBase : IPersistence
    {

        public async Task<int> ExecuteAsync(
        IDbConnection connection,
        string sqlCommand,
        object parameters,
        IDbTransaction transaction,
        CommandType type,
        int? commandTimeout = null)
        {
           return await connection.ExecuteAsync(sqlCommand, parameters, transaction, commandTimeout, type); 
        }

        public async Task<T> ExecuteAsync<T>(
            IDbConnection connection,
            string sqlCommand,
            object parameters,
            IDbTransaction transaction,
            CommandType type,
            int? commandTimeout = null)
        {
            return await connection.QueryFirstAsync<T>(sqlCommand, parameters, transaction, commandTimeout, type); 
        }

        public async Task<IEnumerable<T>> SelectAsync<T>(
            IDbConnection connection,
            string sqlQuery,
            object parameters,
            IDbTransaction transaction,
            CommandType type,
            int? commandTimeout = null)
        {
            return await connection.QueryAsync<T>(sqlQuery, parameters, transaction, commandTimeout, type); 
        }

        public async Task<IEnumerable<T>> SelectAsync<T, S>(
            IDbConnection connection,
            string sqlQuery,
            Func<T,S,T> map,
            string splitOn,
            object parameters,
            IDbTransaction transaction,
            CommandType type,
            bool buffered,
            int? commandTimeout = null)
        {
            return await connection.QueryAsync<T, S, T>(sqlQuery, map, parameters, transaction, buffered, splitOn, commandTimeout, type);
        }

        public async Task<GridReader> SelectMultipleAsync<T>(
        IDbConnection connection,
        string sqlQuery,
        object parameters,
        IDbTransaction transaction,
        CommandType type,
        int? commandTimeout = null)
        {
            return await connection.QueryMultipleAsync(sqlQuery, parameters, transaction, commandTimeout, type);
        }

        public async Task<T> SingleAsync<T>(
            IDbConnection connection,
            string sqlQuery,
            object parameters,
            IDbTransaction transaction,
            CommandType type,
            int? commandTimeout = null)
        {
            return await connection.QuerySingleAsync<T>(sqlQuery, parameters, transaction, commandTimeout, type); 
        }
    }
}