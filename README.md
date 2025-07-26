# Tapper
## _The last library that wraps Dapper, ever._ 
[Dapper](https://github.com/DapperLib/Dapper) is a simple object mapper for .Net. 
What this library is not going to address:
- Is there any need for any abstraction, in the larger sense?
- Does Dappper overcome the IRepository pattern compared to Entity Framework simple implementation?
- Do we need to learn SQL at all? 

## Technology Stack

**Core Library:**
- Target Framework: `netstandard2.1`
- Dependencies: `Dapper 2.1.28`

**Test Project:**
- Target Framework: `net9.0`
- Database Drivers: `MySql.Data 8.3.0`, `Microsoft.Data.SqlClient 5.1.4`
- Test Framework: `NUnit 4.0.1`
- Additional Features: `MediatR 12.2.0`, `Dapper.SqlBuilder 2.0.78`

**Database Support:**
- **MySQL 8.0** (via Docker container on port 3306)
- **Microsoft SQL Server 2022** (via Docker container on port 5434)

## How to
From the root of the solution, where the file "Mahamudra.Tapper.sln" is.
```powershell
docker compose up -d
```

Then from the tests folder: "Mahamudra.Tapper.Tests"
```powershell
dotnet test
```
## Database-Specific Examples

### MySQL Integration Tests
The library supports MySQL with database-specific implementations using `LAST_INSERT_ID()`:

```csharp
[Test]
public async Task ProductCreateCommand_ShouldInsertProduct_WithTransaction()
{
    var authInfo = BasicAuthenticationInfo;
    var expectedProductName = Random.Shared.NextSingle().ToString();
    var expectedPrice = 33.50M;
    var expectedModelYear = (short)DateTime.UtcNow.Year;

    using var context = await _factory.Create(new MySQLTransaction());
    
    // Create category
    var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
        new CategoryCreateCommand(authInfo)
        {
            Name = expectedCategoryName
        }));
    Assert.That(categoryId, Is.GreaterThan(0));

    // Create brand
    var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
        new BrandCreateCommand(authInfo)
        {
            Name = expectedBrandName
        }));
    Assert.That(brandId, Is.GreaterThan(0));

    // Create product
    var command = new ProductCreateCommand(authInfo)
    {
        Name = expectedProductName,
        ListPrice = expectedPrice,
        ModelYear = expectedModelYear,
        BrandId = brandId.Value,
        CategoryId = categoryId.Value
    };

    var productId = await context.Execute(new ProductMySqlCreateCommandPersistence(command));
    Assert.That(productId, Is.GreaterThan(0));
    context.Commit();

    // Verify creation
    var product = await context.Query(new ProductGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
    {
        Id = productId.Value
    }));
    Assert.That(product!.Name, Is.EqualTo(expectedProductName));
}
```

### MSSQL Integration Tests
The library supports SQL Server with `SCOPE_IDENTITY()` for auto-increment IDs:

```csharp
[Test]
public async Task ProductCreateCommand_ShouldCreateMultipleProducts_WithBatch()
{
    var authInfo = BasicAuthenticationInfo;
    
    using var context = await _factory.Create(new MSSQLTransaction());
    
    // Setup dependencies
    var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
        new CategoryCreateCommand(authInfo) { Name = expectedCategoryName }));
    
    var brand = await _handler.Send(new BrandCreateCommand(authInfo) { Name = expectedBrandName });
    
    // Create multiple products in batch
    var products = new List<int?>();
    for (int i = 0; i < 5; i++)
    {
        var command = new ProductCreateCommand(authInfo)
        {
            Name = $"Product_{Random.Shared.NextSingle()}",
            ListPrice = 10M + i,
            ModelYear = (short)DateTime.UtcNow.Year,
            BrandId = brand.Id,
            CategoryId = categoryId.Value
        };

        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
        products.Add(productId);
    }

    context.Commit();
    
    // Verify all products were created
    foreach (var productId in products)
    {
        var product = await context.Query(new ProductGetByIdQueryPersistence(
            new ProductGetByIdQuery(authInfo) { Id = productId.Value }));
        Assert.That(product, Is.Not.Null);
    }
}
```

### Transaction Rollback Testing
```csharp
[Test]
public async Task Transaction_ShouldRollbackOnException()
{
    var authInfo = BasicAuthenticationInfo;
    var expectedCategoryName = Random.Shared.NextSingle().ToString();

    try
    {
        using var context = await _factory.Create(new MSSQLTransaction());
        
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo) { Name = expectedCategoryName }));
        
        throw new InvalidOperationException("Simulated error");
    }
    catch (InvalidOperationException) { }

    // Verify rollback - category should not exist
    using var verifyContext = await _factory.Create();
    var categories = await verifyContext.Query(new BrandCategoryGetAllByQueryPersistence());
    var foundCategory = categories.Categories?.Any(c => c.Name == expectedCategoryName) ?? false;
    Assert.That(foundCategory, Is.False);
}
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