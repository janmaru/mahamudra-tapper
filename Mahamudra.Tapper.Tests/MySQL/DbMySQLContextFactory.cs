using Mahamudra.Tapper.Interfaces;
using MySql.Data.MySqlClient;

namespace Mahamudra.Tapper.Tests.MSSQL;

public class DbMySQLContextFactory(string connectionString, string database) : IDbContextFactory,  
    IProductionMySQLDbContextFactory,
    ISalesMySQLDbContextFactory
{
    private readonly string _db = database;
    private readonly string _connectionString = connectionString;

    public async Task<IDbContext> Create(
        ITransaction transactional,
        CancellationToken ct = default)
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return new DbContext(conn, transactional, _db);
    }
}