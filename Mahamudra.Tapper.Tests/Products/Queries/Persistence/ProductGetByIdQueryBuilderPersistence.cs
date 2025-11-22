using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence;

public class ProductGetByIdQueryBuilderPersistence : DapperBase, IQuery<Product?>
{
    private readonly ProductGetByIdQuery _query;
    private static readonly string _sqlSelect = ProductSQLBuilder.SelectAllById();

    public ProductGetByIdQueryBuilderPersistence(ProductGetByIdQuery query)
    {
        this._query = query;
    }

    public async Task<Product?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        var dto = (await ((IPersistence)this).SelectAsync<ProductDto>(connection!, _sqlSelect.Add(schema), new
        {
            id = _query.Id
        }, transaction))
        .FirstOrDefault();

        return dto?.ToDomain();
    }
}