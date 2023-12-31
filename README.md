# Tapper
## _The last library that wraps Dapper, ever._ 
[Dapper](https://github.com/DapperLib/Dapper) is a simple object mapper for .Net. 
What this library is not going to address:
- Is there any need for any abstraction, in the larger sense?
- Does Dappper overcome the IRepository pattern compared to Entity Framework simple implementation?
- Do we need to learn SQL at all? 

## How to
From the root of the solution, where the file "Mahamudra.Tapper.sln" is.
```powershell
docker compose up -d
```

Then from the tests folder: "Mahamudra.Tapper.Tests"
```powershell
dotnet test
```
## Features

- Commands and queries are logically separated.
```csharp
        using var context = await _factory.Create();
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
```
```csharp
        using var context = await _factory.Create();
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
        Assert.That(productId, Is.GreaterThan(0));
        var product = await context.Query(new ProductGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
        {
             Id = productId.Value
        })); 
        Assert.That(product!.Name, Is.EqualTo(expectedProductName));
```
- Using transactions.
(_Using transactions is not evil in itself, maybe it has no meaning in a world where you can't even synchronize your watch._)
```csharp
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
        Assert.That(productId, Is.GreaterThan(0));
        context.Rollback();
```
```csharp
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
        Assert.That(productId, Is.GreaterThan(0));
        context.Commit();
``` 
- Persistence can be wrapped with any CQRS pattern.
(_MediatR it's a way of implementing CQRS. It's not the only way. And probably is not even implementing the mediator pattern._)
```csharp
        var brand  = await _handler.Send( 
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            });
        Assert.That(brand.Id, Is.GreaterThan(0));
``` 
- Using Dapper.SqlBuilder, a simple sql formatter for .Net
```csharp
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
        var builderTemplate = builder.AddTemplate($"Select /**select**/ from /*schema*/ tableName} /**where**/ ");
        return builderTemplate.RawSql;
    }
```  

- Using stored procedures
```csharp

public class BrandGetByIdQueryPersistence(BrandGetByIdQuery query) : DapperBase, IQuery<Brand?>
{
    private readonly BrandGetByIdQuery _query = query;
    private static readonly string _sqlSelect = @"/*schema*/ [uspGetBrandById]";

    public async Task<Brand?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        return (await ((IPersistence)this).SelectAsync<Brand>(connection!, _sqlSelect.Add(schema), new
        {
            id = _query.Id
        },
        transaction,
        CommandType.StoredProcedure))
        .FirstOrDefault();
    }
}
```  

- Using multiple queries.

```csharp
public class BrandCategoryGetAllByQueryPersistence() : DapperBase, IQuery<BrandCategoryDto?>
{
    private static readonly string _sqlSelect = BrandSQLBuilder.SelectAll() + ";" + CategorySQLBuilder.SelectAll();

    public async Task<BrandCategoryDto?> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
    {
        BrandCategoryDto brandCategoryDto = new();
        var values = await ((IPersistence)this).SelectMultipleAsync<BrandCategoryDto>(connection!, _sqlSelect.Add(schema), null, transaction);

        brandCategoryDto.Brands =  await values.ReadAsync<Brand>();
        brandCategoryDto.Categories = await values.ReadAsync<Category>();
        return brandCategoryDto;
    }
}
```  

- Using One-To-Many Relationships
(_Using split-on and builder._)  
```csharp
        return (await ((IPersistence)this).SelectAsync<Product, Category>(
            connection!,
            sql,
            (product, category) =>
            {
                product.Category = category;
                return product;
            },
            splitOn: $"CategoryId",
            new
            {
                id = _query.Id
            }, transaction))
        .FirstOrDefault();
```  