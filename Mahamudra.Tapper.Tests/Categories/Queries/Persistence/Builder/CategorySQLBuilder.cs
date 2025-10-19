using Dapper;
using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Categories.Queries.Persistence.Builder;

/// <summary>
/// SQL Builder for Category queries - uses CategoryDto for database mapping
/// </summary>
public class CategorySQLBuilder
{
    private static string WhereById()
    {
        CategoryDto? obj = null;
        var fieldId = nameof(@obj.Id).GetColumn<CategoryDto>();
        return $"{fieldId} = @{nameof(@obj.Id)}";
    }

    private static SqlBuilder CreateSelect()
    {
        CategoryDto? obj = null;
        var builder = new SqlBuilder();
        builder.Select(nameof(@obj.Id).GetAsColumn<CategoryDto>());
        builder.Select(nameof(@obj.Name).GetAsColumn<CategoryDto>());
        return builder;
    }

    public static string SelectAll()
    {
        var tableName = "categories"; // Explicit table name
        var builder = CreateSelect();
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from /*schema*/ {tableName}");
        return builderTemplate.RawSql;
    }
} 