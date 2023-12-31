using Dapper;
using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;

public class BrandSQLBuilder
{
    private static string WhereById()
    {
        Brand? obj;
        var fieldId = nameof(@obj.Id).GetColumn<Brand>();
        return $"{fieldId} = @{nameof(@obj.Id)}";
    }

    private static SqlBuilder CreateSelect()
    {
        Brand? @obj = null;
        var builder = new SqlBuilder();
        builder.Select(nameof(@obj.Id).GetAsColumn<Brand>());
        builder.Select(nameof(@obj.Name).GetAsColumn<Brand>()); 
        return builder;
    }

    public static string SelectAll()
    {
        var tableName = nameof(Brand);
        tableName = tableName.GetTable<Brand>();
        var builder = CreateSelect(); 
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from /*schema*/ {tableName}");
        return builderTemplate.RawSql;
    }
} 