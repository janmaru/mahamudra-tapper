using Dapper;
using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Brands.Queries.Persistence.Builder;

/// <summary>
/// SQL Builder for Brand queries - uses BrandDto for database mapping
/// </summary>
public class BrandSQLBuilder
{
    private static string WhereById()
    {
        BrandDto? obj = null;
        var fieldId = nameof(@obj.Id).GetColumn<BrandDto>();
        return $"{fieldId} = @{nameof(@obj.Id)}";
    }

    private static SqlBuilder CreateSelect()
    {
        BrandDto? obj = null;
        var builder = new SqlBuilder();
        builder.Select(nameof(@obj.Id).GetAsColumn<BrandDto>());
        builder.Select(nameof(@obj.Name).GetAsColumn<BrandDto>());
        return builder;
    }

    public static string SelectAll()
    {
        var tableName = "brands"; // Explicit table name
        var builder = CreateSelect();
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from /*schema*/ {tableName}");
        return builderTemplate.RawSql;
    }
} 