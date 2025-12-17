using Mahamudra.Tapper.Interfaces;
using MySql.Data.MySqlClient;

namespace Mahamudra.Tapper.Tests.MySQL;

public class DbMySQLContextFactory  : IDbContextFactory,  
    IProductionMySQLDbContextFactory,
    ISalesMySQLDbContextFactory
{
    private readonly string _db;
    private readonly string _connectionString;

    public DbMySQLContextFactory(string connectionString, string database)
    {
        this._connectionString = connectionString;
        this._db = database;
    }

    public async Task<IDbContext> Create(
        ITransaction transactional,
        CancellationToken ct = default)
    {
        var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return new DbContext(conn, transactional, _db);
    }
}