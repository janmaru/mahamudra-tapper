using Mahamudra.Tapper.Interfaces;
using Microsoft.Data.SqlClient;

namespace Mahamudra.Tapper.Tests.MSSQL;

public class DbMSSQLContextFactor : IDbContextFactory, IProductionDbContextFactory, ISalesDbContextFactory
{
    private readonly string _schema;
    private readonly string _connectionString;
    public DbMSSQLContextFactor(string connectionString, string schema)
    {
        this._schema = schema;
        this._connectionString = connectionString;
    }

    public async Task<IDbContext> Create(
        ITransaction transactional,
        CancellationToken ct = default)
    {
        var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return new DbContext(conn, transactional, _schema);
    }
}