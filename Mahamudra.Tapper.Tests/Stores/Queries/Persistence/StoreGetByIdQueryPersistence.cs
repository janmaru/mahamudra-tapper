using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using System.Data;

namespace Mahamudra.Tapper.Tests.Stores.Queries.Persistence;

/// <summary>
/// MSSQL query persistence for Store with GUID ID
/// </summary>
public class StoreGetByIdQueryPersistence : DapperBase, IQuery<Store?>
{
    private readonly StoreGetByIdQuery _query;
    private static readonly string _sqlSelect = @"/*schema*/ [uspGetStoreById]";

    public StoreGetByIdQueryPersistence(StoreGetByIdQuery query)
    {
        this._query = query;
    }

    public async Task<Store?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        var dto = (await ((IPersistence)this).SelectAsync<StoreDto>(connection!, _sqlSelect.Add(schema), new
        {
            id = _query.Id
        },
        transaction,
        CommandType.StoredProcedure))
        .FirstOrDefault();

        return dto?.ToDomain();
    }
}
