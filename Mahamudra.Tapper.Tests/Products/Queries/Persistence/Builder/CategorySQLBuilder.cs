using Dapper;
using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;

public class CategorySQLBuilder
{
    private static string WhereById()
    {
        Category? obj;
        var fieldId = nameof(@obj.Id).GetColumn<Category>();
        return $"{fieldId} = @{nameof(@obj.Id)}";
    }

    private static SqlBuilder CreateSelect()
    {
        Category? @obj = null;
        var builder = new SqlBuilder();
        builder.Select(nameof(@obj.Id).GetAsColumn<Category>());
        builder.Select(nameof(@obj.Name).GetAsColumn<Category>()); 
        return builder;
    }

    public static string SelectAll()
    {
        var tableName = nameof(Category);
        tableName = tableName.GetTable<Category>();
        var builder = CreateSelect(); 
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from /*schema*/ {tableName}");
        return builderTemplate.RawSql;
    }
} 