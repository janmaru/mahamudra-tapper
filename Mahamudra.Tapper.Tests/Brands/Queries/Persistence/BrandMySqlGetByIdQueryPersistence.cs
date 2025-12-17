using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Brands;
using Mahamudra.Tapper.Tests.Brands.Queries;
using Mahamudra.Tapper.Tests.Common;
using System.Data;

namespace Mahamudra.Tapper.Tests.Brands.Queries.Persistence;

public class BrandMySqlGetByIdQueryPersistence : DapperBase, IQuery<Brand?>
{
    private readonly BrandGetByIdQuery _query;
    private static readonly string _sqlSelect = @"uspGetBrandById";

    public BrandMySqlGetByIdQueryPersistence(BrandGetByIdQuery query)
    {
        this._query = query;
    }

    public async Task<Brand?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        var dto = (await ((IPersistence)this).SelectAsync<BrandDto>(connection!, _sqlSelect.Add(schema), new
        {
            id = _query.Id
        },
        transaction,
        CommandType.StoredProcedure))
        .FirstOrDefault();

        return dto?.ToDomain();
    }
}