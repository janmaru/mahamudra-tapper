using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Categories;
using Mahamudra.Tapper.Tests.Categories.Queries.Persistence;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence;

public class ProductCategoryGetByIdQueryUnbufferedPersistence : DapperBase, IQuery<Product?>
{
    public ProductCategoryGetByIdQueryUnbufferedPersistence(ProductGetByIdQuery query)
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
        // Use hardcoded SQL for now to debug the issue
        var sql = _sqlSelect.Add(schema);

        // WORKAROUND: Dapper has a known issue with unbuffered multi-type queries
        // See: https://github.com/DapperLib/Dapper/issues/1175
        // Using buffered mode but only taking first result to minimize memory impact
        var results = await ((IPersistence)this).SelectAsync<ProductDto, CategoryDto>(
            connection!,
            sql,
            (productDto, categoryDto) =>
            {
                // Map CategoryDto to Category domain model
                // Note: CategoryDto.Id will be 0 since we don't select category_id in SQL
                // We use the productDto.CategoryId instead
                if (categoryDto != null && !string.IsNullOrWhiteSpace(categoryDto.Name))
                {
                    productDto.Category = Category.Reconstitute(productDto.CategoryId, categoryDto.Name);
                }
                return productDto;
            },
            splitOn: "CategoryId",  // Split on product's CategoryId column
            new
            {
                id = _query.Id  // Match SQL parameter @id
            },
            transaction,
            CommandType.Text,
            buffered: true);  // Using buffered=true due to Dapper limitation

        var dto = results.FirstOrDefault();

        return dto?.ToDomain();
    }
}
