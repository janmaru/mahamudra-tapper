using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Dtos;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence;

public class BrandCategoryGetAllByQueryPersistence  : DapperBase, IQuery<BrandCategoryDto?>
{
    private static readonly string _sqlSelect = BrandSQLBuilder.SelectAll() + ";" + CategorySQLBuilder.SelectAll();

    public BrandCategoryGetAllByQueryPersistence()
    {

    }

    public async Task<BrandCategoryDto?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        BrandCategoryDto brandCategoryDto = new();
        var values = await ((IPersistence)this).SelectMultipleAsync<BrandCategoryDto>(connection!, _sqlSelect.Add(schema), null, transaction);

        brandCategoryDto.Brands =  await values.ReadAsync<Brand>();
        brandCategoryDto.Categories = await values.ReadAsync<Category>();
        return brandCategoryDto;
    }
}