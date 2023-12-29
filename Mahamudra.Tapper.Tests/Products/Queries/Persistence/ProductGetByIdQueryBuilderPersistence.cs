using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence;

public class ProductGetByIdQueryBuilderPersistence(ProductGetByIdQuery query) : DapperBase, IQuery<Product?>
{
    private readonly ProductGetByIdQuery _query = query;
    private static readonly string _sqlSelect = ProductSQLBuilder.SelectAllById();

    public async Task<Product?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        return (await ((IPersistence)this).SelectAsync<Product>(connection!, _sqlSelect.Add(schema), new
        {
            id = _query.Id
        }, transaction))
        .FirstOrDefault();
    }
}