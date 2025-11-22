# Tapper
## _The last library that wraps Dapper, ever._

[![NuGet](https://img.shields.io/nuget/v/Mahamudra.Tapper.svg)](https://www.nuget.org/packages/Mahamudra.Tapper)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Mahamudra.Tapper.svg)](https://www.nuget.org/packages/Mahamudra.Tapper)

## Installation

```bash
dotnet add package Mahamudra.Tapper
```

Or via Package Manager:
```powershell
Install-Package Mahamudra.Tapper
```

[Dapper](https://github.com/DapperLib/Dapper) is a simple object mapper for .Net.
What this library is not going to address:
- Is there any need for any abstraction, in the larger sense?
- Does Dappper overcome the IRepository pattern compared to Entity Framework simple implementation?
- Do we need to learn SQL at all?

## Architecture

This project implements **Clean Architecture** with **Domain-Driven Design (DDD)** principles organized in **Vertical Slices**.

### Architectural Principles

**Clean Architecture Layers:**
- **Domain Layer**: Business entities with encapsulated logic and validation
- **Application Layer**: Commands, queries, and handlers (CQRS pattern)
- **Infrastructure Layer**: Database persistence, DTOs, and SQL implementations

**Domain-Driven Design:**
- **Entities**: Immutable creation via factory methods (`Create()`, `Reconstitute()`)
- **Value Objects**: Future support for complex types (Address, Money, etc.)
- **Encapsulation**: Private setters, business logic in methods
- **Validation**: Business rules enforced at entity construction time

**Vertical Slice Architecture:**
- Features organized by domain concept (Brands, Categories, Products, Stores)
- Each slice contains its own Commands, Queries, Persistence, and Handlers
- Promotes high cohesion and low coupling

### Technology Stack

**Core Library:**
- Target Framework: `netstandard2.1`
- Dependencies: `Dapper 2.1.28`

**Test Project:**
- Target Framework: `net9.0`
- Database Drivers: `MySql.Data 8.3.0`, `Microsoft.Data.SqlClient 5.1.4`
- Test Framework: `NUnit 4.0.1`
- Additional Features: `Mediator 2.1.7` (high-performance source generator), `Dapper.SqlBuilder 2.0.78`

**Database Support:**
- **MySQL 8.0** (via Docker container on port 3306)
- **Microsoft SQL Server 2022** (via Docker container on port 5434)

## Project Structure

```
Mahamudra.Tapper.Tests/
├── Brands/                          # Brand Slice
│   ├── Brand.cs                     # Domain Entity (sealed, encapsulated)
│   ├── Commands/
│   │   ├── BrandCreateCommand.cs
│   │   └── Persistence/
│   │       ├── BrandCreateCommandPersistence.cs
│   │       └── BrandMySqlCreateCommandPersistence.cs
│   ├── Queries/
│   │   ├── BrandGetByIdQuery.cs
│   │   └── Persistence/
│   │       ├── BrandDto.cs          # Infrastructure DTO with DB attributes
│   │       ├── BrandGetByIdQueryPersistence.cs
│   │       └── Builder/
│   │           └── BrandSQLBuilder.cs
│   └── CommandHandlers/
│       └── BrandCreateCommandHandler.cs
├── Categories/                      # Category Slice (similar structure)
├── Products/                        # Product Slice (similar structure)
└── Stores/                          # Store Slice (similar structure)
```

### Domain Entity Example

Entities follow DDD principles with encapsulation and validation:

```csharp
public sealed class Brand
{
    private const int MaxNameLength = 255;

    // Private constructor for infrastructure
    private Brand() { }

    // Factory method for creating new entities
    public static Brand Create(string name)
    {
        ValidateName(name);
        return new Brand { Name = name.Trim() };
    }

    // Factory method for reconstituting from database
    internal static Brand Reconstitute(int id, string name)
    {
        return new Brand { Id = id, Name = name };
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    // Business behavior
    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Brand name cannot be empty.");
        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Brand name cannot exceed {MaxNameLength} characters.");
    }
}
```

### DTO Pattern for Infrastructure

DTOs handle database mapping, keeping infrastructure concerns separate from domain:

```csharp
// Infrastructure layer only
[Table("brands")]
internal sealed class BrandDto
{
    [Column("brand_id")]
    public int Id { get; set; }

    [Column("brand_name")]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    // Convert to domain entity
    public Brand ToDomain() => Brand.Reconstitute(Id, Name);
}
```

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
(_Mediator library provides high-performance CQRS implementation using source generators for zero-allocation mediator pattern._)
```csharp
        var brand  = await _handler.Send( 
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            });
        Assert.That(brand.Id, Is.GreaterThan(0));
``` 
- Using Dapper.SqlBuilder, a simple sql formatter for .Net
(_SQL builders now use DTOs for database mapping, maintaining clean separation from domain entities._)
```csharp
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
}
```  

- Using stored procedures
(_Query persistence maps DTOs from database and converts to domain entities._)
```csharp
public class BrandGetByIdQueryPersistence : DapperBase, IQuery<Brand?>
{
    private readonly BrandGetByIdQuery _query;
    private static readonly string _sqlSelect = @"/*schema*/ [uspGetBrandById]";

    public BrandGetByIdQueryPersistence(BrandGetByIdQuery query)
    {
        this._query = query;
    }

    public async Task<Brand?> Select(IDbConnection connection, IDbTransaction transaction,
        CancellationToken ct = default, string? schema = null)
    {
        // Query returns DTO from database
        var dto = (await ((IPersistence)this).SelectAsync<BrandDto>(connection!,
            _sqlSelect.Add(schema),
            new { id = _query.Id },
            transaction,
            CommandType.StoredProcedure))
        .FirstOrDefault();

        // Convert DTO to domain entity
        return dto?.ToDomain();
    }
}
```

- Using multiple queries.
(_DTOs are mapped from database, then converted to domain entities._)
```csharp
public class BrandCategoryGetAllByQueryPersistence : DapperBase, IQuery<BrandCategoryDto?>
{
    private static readonly string _sqlSelect = BrandSQLBuilder.SelectAll() + ";" + CategorySQLBuilder.SelectAll();

    public async Task<BrandCategoryDto?> Select(IDbConnection connection, IDbTransaction transaction,
        CancellationToken ct = default, string? schema = null)
    {
        BrandCategoryDto brandCategoryDto = new();
        var values = await ((IPersistence)this).SelectMultipleAsync<BrandCategoryDto>(
            connection!, _sqlSelect.Add(schema), null, transaction);

        // Read DTOs and convert to domain entities
        var brandDtos = await values.ReadAsync<BrandDto>();
        var categoryDtos = await values.ReadAsync<CategoryDto>();

        brandCategoryDto.Brands = brandDtos.Select(dto => dto.ToDomain());
        brandCategoryDto.Categories = categoryDtos.Select(dto => dto.ToDomain());
        return brandCategoryDto;
    }
}
```  

- Using One-To-Many Relationships
(_Using split-on and builder with DTO mapping._)
```csharp
public async Task<Product?> Select(IDbConnection connection, IDbTransaction transaction,
    CancellationToken ct = default, string? schema = null)
{
    var sql = ProductSQLBuilder.SelectWithCategoryAllById().Add(schema);

    // Map to DTOs first, then convert to domain entities
    var dto = (await ((IPersistence)this).SelectAsync<ProductDto, Category>(
        connection!,
        sql,
        (productDto, category) =>
        {
            productDto.Category = category;
            return productDto;
        },
        splitOn: $"CategoryId",
        new { id = _query.Id },
        transaction))
    .FirstOrDefault();

    return dto?.ToDomain();
}
```

## Benefits of This Architecture

### Clean Architecture Advantages
- **Testability**: Domain logic isolated from infrastructure concerns
- **Maintainability**: Clear separation of concerns makes code easier to understand and modify
- **Flexibility**: Can swap database implementations without changing domain entities
- **Database Independence**: Domain entities are free from ORM attributes

### DDD Implementation Benefits
- **Encapsulation**: Entities protect business invariants through private setters
- **Validation**: Business rules enforced at entity creation time via factory methods
- **Immutability**: Entities cannot be created in invalid states
- **Rich Domain Model**: Behavior lives with the data in domain entities

### Vertical Slice Organization
- **Feature Cohesion**: Related code stays together (Commands, Queries, Persistence)
- **Low Coupling**: Features are independent and self-contained
- **Easier Navigation**: Find all related code for a feature in one place
- **Scalability**: Easy to add new features without affecting existing ones

### DTO Pattern Benefits
- **Clear Boundaries**: Infrastructure DTOs keep database concerns separate
- **Type Safety**: Compile-time checking of database mappings
- **Flexibility**: Can change database schema without affecting domain
- **Performance**: Efficient mapping with Dapper's micro-ORM approach

## Performance Best Practices

### Batch Operations
For high-volume insert or update operations, use `ExecuteBatchAsync` to reduce network round-trips. This method executes the same command for a collection of parameters.

```csharp
public async Task<int> ExecuteBatch(IDbConnection connection, IEnumerable<ProductCreateCommand> commands, IDbTransaction transaction)
{
    var sql = @"INSERT INTO products (name, price) VALUES (@Name, @Price)";
    // Executes the SQL once for each item in the collection, but optimized by Dapper
    return await ((IPersistence)this).ExecuteBatchAsync(connection, sql, commands, transaction);
}
```

### Memory Management (Buffering)
By default, `SelectAsync` buffers all results in memory (`buffered: true`). For large datasets, set `buffered: false` to stream results and reduce memory pressure.

```csharp
public async Task<IEnumerable<ProductDto>> GetAllProductsStream(IDbConnection connection, IDbTransaction transaction)
{
    // buffered: false returns an open IDataReader wrapped as IEnumerable
    // The connection must remain open while iterating
    return await ((IPersistence)this).SelectAsync<ProductDto>(
        connection, 
        "SELECT * FROM products", 
        null, 
        transaction, 
        CommandType.Text, 
        buffered: false);
}
```

### Connection Pooling
The library relies on the underlying ADO.NET provider for connection pooling. Ensure your connection strings are configured correctly:

**SQL Server:**
```
Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;Pooling=true;Min Pool Size=5;Max Pool Size=100;
```

**MySQL:**
```
Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;Pooling=true;MinimumPoolSize=5;MaximumPoolSize=100;
```

Always dispose `DbContext` (or `IDbConnection`) promptly to return connections to the pool. The `using` statement is recommended.

## Performance Analysis

For a comprehensive analysis of performance considerations, optimization opportunities, and best practices for high-performance scenarios, see the [Performance Analysis Document](PERFORMANCE_ANALYSIS.md).

This document covers:
- Batch operation patterns
- Memory management with buffering
- Query caching strategies
- Connection pooling configuration
- Identified improvements and their implementation status

## Releases

This project uses [GitVersion](https://gitversion.net/) for semantic versioning. Releases are automated via GitHub Actions.

### Creating a Release

To publish a new version to NuGet:

1. Create and push a git tag:
```bash
git tag v1.0.2
git push origin v1.0.2
```

2. The GitHub Action will automatically:
   - Build and test the project
   - Pack the NuGet package
   - Publish to [NuGet.org](https://www.nuget.org/packages/Mahamudra.Tapper)

### Version Scheme

- **Patch** (1.0.x): Bug fixes and minor improvements
- **Minor** (1.x.0): New features, backward compatible
- **Major** (x.0.0): Breaking changes