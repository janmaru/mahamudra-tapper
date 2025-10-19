using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Categories;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Queries;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence;

public class ProductCategoryGetByIdQueryPersistence  : DapperBase, IQuery<Product?>
{
    public ProductCategoryGetByIdQueryPersistence(ProductGetByIdQuery query)
    {
        this._query = query;
    }

    private readonly ProductGetByIdQuery _query;
    private static readonly string _sqlSelect = @"
SELECT product_id    AS Id,
       product_name  AS NAME,
       brand_id      AS BrandId,
       model_year    AS ModelYear,
       list_price    AS ListPrice, 
       p.category_id AS CategoryId,
       category_name AS NAME
FROM   /*schema*/ products p
       INNER JOIN /*schema*/ categories c
               ON p.category_id = c.category_id
WHERE  product_id = @id; 
";
    private static readonly string _sqlSelectWithBuilder = ProductSQLBuilder.SelectWithCategoryAllById();
    public async Task<Product?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        var sql = _sqlSelectWithBuilder.Add(schema);
        // or choose
        // _sqlSelect.Add(schema)
        var dto = (await ((IPersistence)this).SelectAsync<ProductDto, Category>(
            connection!,
            sql,
            (productDto, category) =>
            {
                productDto.Category = category;
                return productDto;
            },
            splitOn: $"CategoryId",
            new
            {
                id = _query.Id
            }, transaction))
        .FirstOrDefault();

        return dto?.ToDomain();
    }
}