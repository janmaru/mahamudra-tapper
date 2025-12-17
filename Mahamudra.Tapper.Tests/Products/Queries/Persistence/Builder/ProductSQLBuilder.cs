using Dapper;
using Mahamudra.Tapper.Tests.Categories.Queries.Persistence;
using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;

/// <summary>
/// SQL Builder for Product queries - uses ProductDto for database mapping
/// </summary>
public class ProductSQLBuilder
{
    private static string WhereById()
    {
        ProductDto? obj = null;
        var fieldId = nameof(@obj.Id).GetColumn<ProductDto>();
        return $"{fieldId} = @{nameof(@obj.Id)}";
    }

    private static SqlBuilder CreateSelect()
    {
        ProductDto? obj = null;
        var builder = new SqlBuilder();
        builder.Select(nameof(@obj.Id).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.Name).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.BrandId).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.CategoryId).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.ModelYear).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.ListPrice).GetAsColumn<ProductDto>());
        return builder;
    }

    public static string SelectAllById()
    {
        var tableName = "products"; // Explicit table name
        var builder = CreateSelect();
        var where = WhereById();
        builder.Where(where);
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from /*schema*/ {tableName} /**where**/ ");
        return builderTemplate.RawSql;
    }

    private static SqlBuilder CreateSelectWithCategory()
    {
        ProductDto? obj = null;
        var builder = new SqlBuilder();
        builder.Select(nameof(@obj.Id).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.Name).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.BrandId).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.ModelYear).GetAsColumn<ProductDto>());
        builder.Select(nameof(@obj.ListPrice).GetAsColumn<ProductDto>());
        builder.Select($"p.{nameof(@obj.CategoryId).GetAsColumn<ProductDto>()}");

        CategoryDto? categoryObj = null;
        builder.Select(nameof(@categoryObj.Name).GetAsColumn<CategoryDto>());
        return builder;
    }

    public static string SelectWithCategoryAllById()
    {
        var tableName = "/*schema*/ products p"; // Explicit table name
        var innerTableName = "/*schema*/ categories"; // Explicit table name

        var builder = CreateSelectWithCategory();
        CategoryDto? categoryObj = null;
        var id = $"{nameof(@categoryObj.Id).GetColumn<CategoryDto>()}";
        builder.InnerJoin($"{innerTableName} c on p.{id} = c.{id}");
        var where = WhereById();
        builder.Where(where);
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from {tableName} /**innerjoin**/ /**where**/ ");
        return builderTemplate.RawSql;
    }
} 