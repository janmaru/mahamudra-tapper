using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Commands;
using Mahamudra.Tapper.Tests.Products.Commands.Persistence;
using Mahamudra.Tapper.Tests.Products.Queries;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Mahamudra.Tapper.Tests.MySQL;

public class TestsDbMySQLContext
{
    private IDbContextFactory _factory;
    private ILogger<TestsDbMySQLContext> _logger;
    private IMediator _handler;


    private static IAuthenticationInfo BasicAuthenticationInfo => new AuthenticationInfo()
    {
        Id = Guid.NewGuid().ToString()
    };

    [SetUp]
    public void Setup()
    {
        _factory = ServicesProvider.GetRequiredService<IProductionMySQLDbContextFactory>();
        _logger = ServicesProvider.GetRequiredService<ILogger<TestsDbMySQLContext>>();
        _handler = ServicesProvider.GetRequiredService<IMediator>();
    }
  
    [Test]
    public async Task BrandGetByIdQuery_ShouldGetBrand_WithStoreProcedure()
    {
        var authInfo = BasicAuthenticationInfo; 
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create(new MySQLTransaction()); 
        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));
        context.Commit();

        var brand = await context.Query(new BrandMySqlGetByIdQueryPersistence(new BrandGetByIdQuery(authInfo)
        {
             Id = brandId.Value
        }));  
 
        Assert.That(brand!.Name, Is.EqualTo(expectedBrandName));
        Assert.That(brand!.Id, Is.EqualTo(brandId.Value));
    }

    [Test]
    public async Task ProductCreateCommand_ShouldInsertProduct_WithIdResult()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 15.75M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create();
        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));

        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));

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
    }

    [Test]
    public async Task ProductCreateCommand_ShouldInsertProduct_WithSelectResult()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 25.99M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create();
        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));

        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));

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

        var product = await context.Query(new ProductGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
        {
            Id = productId.Value
        }));

        Assert.That(product!.Name, Is.EqualTo(expectedProductName));
        Assert.That(product!.ListPrice, Is.EqualTo(expectedPrice));
        Assert.That(product!.ModelYear, Is.EqualTo(expectedModelYear));
    }

    [Test]
    public async Task ProductCreateCommand_ShouldInsertProduct_WithTransaction()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 33.50M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));

        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));

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

        var product = await context.Query(new ProductGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
        {
            Id = productId.Value
        }));
        Assert.That(product!.Name, Is.EqualTo(expectedProductName));
        Assert.That(product!.ListPrice, Is.EqualTo(expectedPrice));
        Assert.That(product!.ModelYear, Is.EqualTo(expectedModelYear));
    }

    [Test]
    public async Task ProductCreateCommand_ShouldInsertProduct_WithCommandHandler()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 42.25M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));

        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));

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

        var product = await context.Query(new ProductGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
        {
            Id = productId.Value
        }));
        Assert.That(product!.Name, Is.EqualTo(expectedProductName));
        Assert.That(product!.ListPrice, Is.EqualTo(expectedPrice));
        Assert.That(product!.ModelYear, Is.EqualTo(expectedModelYear));
    }

    [Test]
    public async Task ProductCreateCommand_ShouldFailsInsertProduct_WithTransaction()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 55.00M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));

        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));

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
        context.Rollback();

        var product = await context.Query(new ProductGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
        {
            Id = productId.Value
        }));
        Assert.That(product, Is.Null);
    }

    [Test]
    public async Task ProductCreateCommand_ShouldFailsInsertProduct_WithSQLBuilder()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 67.75M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));

        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));

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
        context.Rollback();

        var product = await context.Query(new ProductGetByIdQueryBuilderPersistence(new ProductGetByIdQuery(authInfo)
        {
            Id = productId.Value
        }));
        Assert.That(product, Is.Null);
    }

    [Test]
    public async Task BrandAndCategoryAllQuery_ShouldGetBrandCategoryDto_WithMultipleQuery()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MySQLTransaction());
        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedName
            }));
        Assert.That(brandId, Is.GreaterThan(0));

        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
        context.Commit();

        var categoryBrand = await context.Query(new BrandCategoryGetAllByQueryPersistence());
        Assert.That(categoryBrand, Is.Not.Null);
        Assert.That(categoryBrand.Brands, Is.Not.Null);
        Assert.That(categoryBrand.Categories, Is.Not.Null);

        var expecteBrands = categoryBrand.Brands.Where(x => x.Name == expectedName).ToList();
        var expecteCategories = categoryBrand.Categories.Where(x => x.Name == expectedName).ToList();
        Assert.That(expecteBrands.Count(), Is.EqualTo(1));
        Assert.That(expecteCategories.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task ProductCategoryGetByIdQueryPersistence_ShouldSucceed_WithInnerJoin()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 88.50M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        // Create dependencies without transaction first
        int? categoryId;
        int? brandId;
        
        using (var setupContext = await _factory.Create())
        {
            categoryId = await setupContext.Execute(new CategoryMySqlCreateCommandPersistence(
                new CategoryCreateCommand(authInfo)
                {
                    Name = expectedCategoryName
                }));
            Assert.That(categoryId, Is.GreaterThan(0));

            brandId = await setupContext.Execute(new BrandMySqlCreateCommandPersistence(
                new BrandCreateCommand(authInfo)
                {
                    Name = expectedBrandName
                }));
            Assert.That(brandId, Is.GreaterThan(0));
        }

        // Now create product with transaction
        using var context = await _factory.Create(new MySQLTransaction());
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

        var product = await context.Query(new ProductCategoryGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
        {
            Id = productId.Value
        }));
        Assert.That(product, Is.Not.Null);
        Assert.That(product.Category, Is.Not.Null);
        Assert.That(product.Category.Name, Is.EqualTo(expectedCategoryName));
    }

    [Test]
    public async Task CategoryCreateCommand_ShouldCreateCategory_WithoutTransaction()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedCategoryName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create();
        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));

        Assert.That(categoryId, Is.GreaterThan(0));
    }

    [Test]
    public async Task BrandCreateCommand_ShouldCreateBrand_WithMySQLSpecificImplementation()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MySQLTransaction());
        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        
        Assert.That(brandId, Is.GreaterThan(0));
        context.Commit();

        var brand = await context.Query(new BrandMySqlGetByIdQueryPersistence(new BrandGetByIdQuery(authInfo)
        {
            Id = brandId.Value
        }));

        Assert.That(brand, Is.Not.Null);
        Assert.That(brand.Name, Is.EqualTo(expectedBrandName));
        Assert.That(brand.Id, Is.EqualTo(brandId.Value));
    }

    [Test]
    public async Task DatabaseConnection_ShouldConnect_WithoutErrors()
    {
        using var context = await _factory.Create();
        Assert.That(context, Is.Not.Null);
    }

    [Test]
    public async Task DatabaseConnection_ShouldConnect_WithTransaction()
    {
        using var context = await _factory.Create(new MySQLTransaction());
        Assert.That(context, Is.Not.Null);
        context.Commit();
    }

    [Test]
    public async Task BrandCreateCommand_ShouldReturnValidId_Debug()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MySQLTransaction());
        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        
        Console.WriteLine($"Brand ID returned: {brandId}");
        Assert.That(brandId, Is.Not.Null);
        Assert.That(brandId, Is.GreaterThan(0));
        
        // Verify the brand exists before committing
        var brand = await context.Query(new BrandMySqlGetByIdQueryPersistence(new BrandGetByIdQuery(authInfo)
        {
            Id = brandId.Value
        }));
        
        Assert.That(brand, Is.Not.Null);
        Assert.That(brand.Name, Is.EqualTo(expectedBrandName));
        
        context.Commit();
    }

    [Test]
    public async Task ProductCreateCommand_DebugForeignKeyIssue()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 55.00M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MySQLTransaction());
        
        var categoryId = await context.Execute(new CategoryMySqlCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Console.WriteLine($"Category ID: {categoryId}");
        Assert.That(categoryId, Is.GreaterThan(0));

        var brandId = await context.Execute(new BrandMySqlCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Console.WriteLine($"Brand ID: {brandId}");
        Assert.That(brandId, Is.GreaterThan(0));

        Console.WriteLine($"About to create product with BrandId: {brandId.Value}, CategoryId: {categoryId.Value}");
        
        var command = new ProductCreateCommand(authInfo)
        {
            Name = expectedProductName,
            ListPrice = expectedPrice,
            ModelYear = expectedModelYear,
            BrandId = brandId.Value,
            CategoryId = categoryId.Value
        };

        var productId = await context.Execute(new ProductMySqlCreateCommandPersistence(command));
        Console.WriteLine($"Product ID: {productId}");
        
        context.Commit();
    }
}