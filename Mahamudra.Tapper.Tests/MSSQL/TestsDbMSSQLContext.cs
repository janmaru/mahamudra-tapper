using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Commands;
using Mahamudra.Tapper.Tests.Products.Commands.Persistence;
using Mahamudra.Tapper.Tests.Products.Queries;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mahamudra.Tapper.Tests.MSSQL;

public class TestsDbMSSQLContext
{
    private IDbContextFactory _factory;
    private ILogger<TestsDbMSSQLContext> _logger;
    private IMediator _handler;


    private static IAuthenticationInfo BasicAuthenticationInfo => new AuthenticationInfo()
    {
        Id = Guid.NewGuid().ToString()
    };

    [SetUp]
    public void Setup()
    {
        _factory = ServicesProvider.GetRequiredService<IProductionDbContextFactory>();
        _logger = ServicesProvider.GetRequiredService<ILogger<TestsDbMSSQLContext>>();
        _handler = ServicesProvider.GetRequiredService<IMediator>();
    }

    [Test]
    public async Task ProductCreateCommand_ShouldInsertProduct_WithIdResult()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 10.5M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;
        var command = new ProductCreateCommand(authInfo)
        {
            Name = expectedProductName,
            ListPrice = expectedPrice,
            ModelYear = expectedModelYear
        };

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create();
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
        var brandId = await context.Execute(new BrandCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));
        command.BrandId = brandId.Value;
        command.CategoryId = categoryId.Value;
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
        Assert.That(productId, Is.GreaterThan(0));
    }

    [Test]
    public async Task ProductCreateCommand_ShouldInsertProduct_WithSelectResult()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 10.5M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;
        var command = new ProductCreateCommand(authInfo)
        {
            Name = expectedProductName,
            ListPrice = expectedPrice,
            ModelYear = expectedModelYear
        };

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create();
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
        var brandId = await context.Execute(new BrandCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));
        command.BrandId = brandId.Value;
        command.CategoryId = categoryId.Value;
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
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
        var expectedPrice = 10.5M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;
        var command = new ProductCreateCommand(authInfo)
        {
            Name = expectedProductName,
            ListPrice = expectedPrice,
            ModelYear = expectedModelYear
        };

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
        var brandId = await context.Execute(new BrandCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));
        command.BrandId = brandId.Value;
        command.CategoryId = categoryId.Value;
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
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
        var expectedPrice = 10.5M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;
        var command = new ProductCreateCommand(authInfo)
        {
            Name = expectedProductName,
            ListPrice = expectedPrice,
            ModelYear = expectedModelYear
        };

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
        var brandId = await context.Execute(new BrandCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));
        command.BrandId = brandId.Value;
        command.CategoryId = categoryId.Value;
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
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
        var expectedPrice = 10.5M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;
        var command = new ProductCreateCommand(authInfo)
        {
            Name = expectedProductName,
            ListPrice = expectedPrice,
            ModelYear = expectedModelYear
        };

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
        var brand  = await _handler.Send( 
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            });
        Assert.That(brand.Id, Is.GreaterThan(0));
        command.BrandId = brand.Id;
        command.CategoryId = categoryId.Value;
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
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
        var expectedPrice = 10.5M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;
        var command = new ProductCreateCommand(authInfo)
        {
            Name = expectedProductName,
            ListPrice = expectedPrice,
            ModelYear = expectedModelYear
        };

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
        var brand = await _handler.Send(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            });
        Assert.That(brand.Id, Is.GreaterThan(0));
        command.BrandId = brand.Id;
        command.CategoryId = categoryId.Value;
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
        Assert.That(productId, Is.GreaterThan(0));
        context.Rollback();

        var product = await context.Query(new ProductGetByIdQueryBuilderPersistence(new ProductGetByIdQuery(authInfo)
        {
            Id = productId.Value
        }));
        Assert.That(product, Is.Null);
    }

    [Test]
    public async Task BrandGetByIdQuery_ShouldGetBrand_WithStoreProcedure()
    {
        var authInfo = BasicAuthenticationInfo; 
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create(new MySQLTransaction()); 
        var brandId = await context.Execute(new BrandCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            }));
        Assert.That(brandId, Is.GreaterThan(0));
        context.Commit();

        var brand = await context.Query(new BrandGetByIdQueryPersistence(new BrandGetByIdQuery(authInfo)
        {
             Id = brandId.Value
        }));  
 
        Assert.That(brand!.Name, Is.EqualTo(expectedBrandName));
        Assert.That(brand!.Id, Is.EqualTo(brandId.Value));
    }

    [Test]
    public async Task BrandAndCategoryAllQuery_ShouldGetBrandCategoryDto_WithMultipleQuery()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create(new MySQLTransaction());
        var brandId = await context.Execute(new BrandCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedName
            }));
        Assert.That(brandId, Is.GreaterThan(0));
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
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
        var expecteBrands = categoryBrand.Brands.Where(x=>x.Name == expectedName).ToList();
        var expecteCategories = categoryBrand.Categories.Where(x => x.Name == expectedName).ToList();
        Assert.That(expecteBrands.Count(), Is.EqualTo(1));
        Assert.That(expecteCategories.Count(), Is.EqualTo(1));
    }


    [Test]
    public async Task ProductCategoryGetByIdQueryPersistence_ShouldSucceed_WithInnerJoin()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedProductName = Random.Shared.NextSingle().ToString();
        var expectedPrice = 10.5M;
        var expectedModelYear = (short)DateTime.UtcNow.Year;
        var command = new ProductCreateCommand(authInfo)
        {
            Name = expectedProductName,
            ListPrice = expectedPrice,
            ModelYear = expectedModelYear
        };

        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();
        using var context = await _factory.Create(new MySQLTransaction());
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));
        Assert.That(categoryId, Is.GreaterThan(0));
        var brand = await _handler.Send(
            new BrandCreateCommand(authInfo)
            {
                Name = expectedBrandName
            });
        Assert.That(brand.Id, Is.GreaterThan(0));
        command.BrandId = brand.Id;
        command.CategoryId = categoryId.Value;
        var productId = await context.Execute(new ProductCreateCommandPersistence(command));
        Assert.That(productId, Is.GreaterThan(0));
        context.Commit();

        var product = await context.Query(new ProductCategoryGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
        {
            Id = productId.Value
        }));
        Assert.That(product, Is.Not.Null);
        Assert.That(product.Category, Is.Not.Null);
        Assert.That(product.Category.Name,  Is.EqualTo(expectedCategoryName));
    }
}