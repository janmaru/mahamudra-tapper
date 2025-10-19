using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using System.Data;
using Mahamudra.Tapper.Tests.Stores;
using Mahamudra.Tapper.Tests.Stores.Queries;

namespace Mahamudra.Tapper.Tests.Stores.Queries.Persistence;

/// <summary>
/// MySQL query persistence for Store with GUID ID
/// </summary>
public class StoreMySqlGetByIdQueryPersistence : DapperBase, IQuery<Store?>
{
    private readonly StoreGetByIdQuery _query;
    private static readonly string _sqlSelect = @"uspGetStoreById";

    public StoreMySqlGetByIdQueryPersistence(StoreGetByIdQuery query)
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
