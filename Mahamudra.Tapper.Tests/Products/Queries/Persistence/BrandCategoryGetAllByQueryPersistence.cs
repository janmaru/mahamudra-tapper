using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Brands;
using Mahamudra.Tapper.Tests.Brands.Queries.Persistence;
using Mahamudra.Tapper.Tests.Brands.Queries.Persistence.Builder;
using Mahamudra.Tapper.Tests.Categories;
using Mahamudra.Tapper.Tests.Categories.Queries.Persistence;
using Mahamudra.Tapper.Tests.Categories.Queries.Persistence.Builder;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Dtos;
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

        var brandDtos = await values.ReadAsync<BrandDto>();
        var categoryDtos = await values.ReadAsync<CategoryDto>();

        brandCategoryDto.Brands = brandDtos.Select(dto => dto.ToDomain());
        brandCategoryDto.Categories = categoryDtos.Select(dto => dto.ToDomain());
        return brandCategoryDto;
    }
}