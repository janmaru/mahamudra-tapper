using Dapper;
using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Queries.Persistence.Builder;

public class ProductSQLBuilder
{
    private static string WhereById()
    {
        Product? obj;
        var fieldId = nameof(@obj.Id).GetColumn<Product>();
        return $"{fieldId} = @{nameof(@obj.Id)}";
    }

    private static SqlBuilder CreateSelect()
    {
        Product? @obj = null;
        var builder = new SqlBuilder();
        builder.Select(nameof(@obj.Id).GetAsColumn<Product>());
        builder.Select(nameof(@obj.Name).GetAsColumn<Product>());
        builder.Select(nameof(@obj.BrandId).GetAsColumn<Product>());
        builder.Select(nameof(@obj.CategoryId).GetAsColumn<Product>());
        builder.Select(nameof(@obj.ModelYear).GetAsColumn<Product>());
        builder.Select(nameof(@obj.ListPrice).GetAsColumn<Product>());
        return builder;
    }

    public static string SelectAllById()
    {
        var tableName = nameof(Product);
        tableName = tableName.GetTable<Product>();
        var builder = CreateSelect();
        var where = WhereById();
        builder.Where(where);
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from /*schema*/ {tableName} /**where**/ ");
        return builderTemplate.RawSql;
    }

    private static SqlBuilder CreateSelectWithCategory()
    {
        Product? @obj = null;
        var builder = new SqlBuilder();
        builder.Select(nameof(@obj.Id).GetAsColumn<Product>());
        builder.Select(nameof(@obj.Name).GetAsColumn<Product>());
        builder.Select(nameof(@obj.BrandId).GetAsColumn<Product>());
        builder.Select(nameof(@obj.ModelYear).GetAsColumn<Product>());
        builder.Select(nameof(@obj.ListPrice).GetAsColumn<Product>()); 
        builder.Select($"p.{nameof(@obj.CategoryId).GetAsColumn<Product>()}");
        builder.Select(nameof(Category.Name).GetAsColumn<Category>());
        return builder;
    }
    public static string SelectWithCategoryAllById()
    {
        var tableName = nameof(Product);
        tableName = $"/*schema*/ {tableName.GetTable<Product>()} p";
        var innerTableName = nameof(Category);
        innerTableName = $"/*schema*/ {innerTableName.GetTable<Category>()}";  
        var builder = CreateSelectWithCategory();  
        var id = $"{nameof(Category.Id).GetColumn<Category>()}";
        builder.InnerJoin($"{innerTableName} c on p.{id} = c.{id}");
        var where = WhereById();
        builder.Where(where);
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from {tableName} /**innerjoin**/ /**where**/ ");
        return builderTemplate.RawSql;
    }
} 