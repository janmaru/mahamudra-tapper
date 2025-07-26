using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence;

public class ProductGetByIdQueryPersistence : DapperBase, IQuery<Product?>
{
    public ProductGetByIdQueryPersistence(ProductGetByIdQuery query)
    {
        this._query = query;
    }

    private readonly ProductGetByIdQuery _query;
    private static readonly string _sqlSelect = @"
SELECT product_id as id,
       product_name as name,
       brand_id as brandId,
       category_id as categoryId,
       model_year as modelYear,
       list_price as listPrice
FROM   /*schema*/ products 
WHERE product_id=@id
";

    public async Task<Product?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        return (await ((IPersistence)this).SelectAsync<Product>(connection!, _sqlSelect.Add(schema), new
        {
            id = _query.Id
        }, transaction))
        .FirstOrDefault();
    }
}