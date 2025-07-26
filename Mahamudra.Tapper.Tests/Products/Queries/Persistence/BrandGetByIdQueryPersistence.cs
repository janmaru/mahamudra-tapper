using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence;

public class BrandGetByIdQueryPersistence  : DapperBase, IQuery<Brand?>
{
    private readonly BrandGetByIdQuery _query;
    private static readonly string _sqlSelect = @"/*schema*/ [uspGetBrandById]";

    public BrandGetByIdQueryPersistence(BrandGetByIdQuery query)
    {
        this._query = query;
    }

    public async Task<Brand?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        return (await ((IPersistence)this).SelectAsync<Brand>(connection!, _sqlSelect.Add(schema), new
        {
            id = _query.Id
        },
        transaction,
        CommandType.StoredProcedure))
        .FirstOrDefault();
    }
}