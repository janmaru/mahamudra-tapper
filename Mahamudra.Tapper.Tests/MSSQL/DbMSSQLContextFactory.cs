using Mahamudra.Tapper.Interfaces;
using Microsoft.Data.SqlClient;

namespace Mahamudra.Tapper.Tests.MSSQL;

public class DbMSSQLContextFactory(string connectionString, string schema) : IDbContextFactory, IProductionDbContextFactory, ISalesDbContextFactory
{
    private readonly string _schema = schema;
    private readonly string _connectionString = connectionString;

    public async Task<IDbContext> Create(
        ITransaction transactional,
        CancellationToken ct = default)
    {
        var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return new DbContext(conn, transactional, _schema);
    }
}