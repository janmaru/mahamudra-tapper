using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.MySQL;
using Mahamudra.Tapper.Tests.Brands.Commands;
using Mahamudra.Tapper.Tests.Categories.Commands;
using Mahamudra.Tapper.Tests.Products.Commands;
using Mahamudra.Tapper.Tests.Stores.Commands;
using Mahamudra.Tapper.Tests.Brands.Commands.Persistence;
using Mahamudra.Tapper.Tests.Categories.Commands.Persistence;
using Mahamudra.Tapper.Tests.Products.Commands.Persistence;
using Mahamudra.Tapper.Tests.Stores.Commands.Persistence;
using Mahamudra.Tapper.Tests.Brands.Queries;
using Mahamudra.Tapper.Tests.Categories.Queries;
using Mahamudra.Tapper.Tests.Products.Queries;
using Mahamudra.Tapper.Tests.Stores.Queries;
using Mahamudra.Tapper.Tests.Brands.Queries.Persistence;
using Mahamudra.Tapper.Tests.Categories.Queries.Persistence;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence;
using Mahamudra.Tapper.Tests.Stores.Queries.Persistence;
using Mediator;
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
        using var context = await _factory.Create(new MSSQLTransaction());
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
        using var context = await _factory.Create(new MSSQLTransaction());
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
        using var context = await _factory.Create(new MSSQLTransaction());
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
        using var context = await _factory.Create(new MSSQLTransaction());
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
        using var context = await _factory.Create(new MSSQLTransaction()); 
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
        using var context = await _factory.Create(new MSSQLTransaction());
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
        using var context = await _factory.Create(new MSSQLTransaction());
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

    [Test]
    public async Task CategoryCreateCommand_ShouldCreateCategory_WithoutTransaction()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedCategoryName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create();
        var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
            new CategoryCreateCommand(authInfo)
            {
                Name = expectedCategoryName
            }));

        Assert.That(categoryId, Is.GreaterThan(0));
    }

    [Test]
    public async Task BrandCreateCommand_ShouldCreateBrand_WithTransaction()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MSSQLTransaction());
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
        using var context = await _factory.Create(new MSSQLTransaction());
        Assert.That(context, Is.Not.Null);
        context.Commit();
    }

    [Test]
    public async Task ProductCreateCommand_ShouldCreateMultipleProducts_WithBatch()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedCategoryName = Random.Shared.NextSingle().ToString();
        var expectedBrandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create(new MSSQLTransaction());
        
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
            Assert.That(productId, Is.GreaterThan(0));
            products.Add(productId);
        }

        context.Commit();

        foreach (var productId in products)
        {
            var product = await context.Query(new ProductGetByIdQueryPersistence(new ProductGetByIdQuery(authInfo)
            {
                Id = productId.Value
            }));
            Assert.That(product, Is.Not.Null);
        }
    }

    [Test]
    public async Task Transaction_ShouldRollbackOnException()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedCategoryName = Random.Shared.NextSingle().ToString();

        try
        {
            using var context = await _factory.Create(new MSSQLTransaction());
            
            var categoryId = await context.Execute(new CategoryCreateCommandPersistence(
                new CategoryCreateCommand(authInfo)
                {
                    Name = expectedCategoryName
                }));
            Assert.That(categoryId, Is.GreaterThan(0));

            throw new InvalidOperationException("Simulated error");
        }
        catch (InvalidOperationException)
        {
        }

        using var verifyContext = await _factory.Create();
        var categories = await verifyContext.Query(new BrandCategoryGetAllByQueryPersistence());
        var foundCategory = categories.Categories?.Any(c => c.Name == expectedCategoryName) ?? false;
        Assert.That(foundCategory, Is.False);
    }

    [Test]
    public async Task BrandCreateCommand_ShouldHandleDuplicateNames()
    {
        var authInfo = BasicAuthenticationInfo;
        var brandName = Random.Shared.NextSingle().ToString();

        using var context = await _factory.Create();

        var firstBrandId = await context.Execute(new BrandCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = brandName
            }));
        Assert.That(firstBrandId, Is.GreaterThan(0));

        var secondBrandId = await context.Execute(new BrandCreateCommandPersistence(
            new BrandCreateCommand(authInfo)
            {
                Name = brandName
            }));
        Assert.That(secondBrandId, Is.GreaterThan(0));
        Assert.That(secondBrandId, Is.Not.EqualTo(firstBrandId));
    }

    [Test]
    public async Task StoreCreateCommand_ShouldInsertStore_WithGuidId()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedStoreName = $"Store_{Random.Shared.NextSingle()}";
        var expectedPhone = "(555) 123-4567";
        var expectedEmail = "store@example.com";
        var expectedStreet = "123 Main St";
        var expectedCity = "New York";
        var expectedState = "NY";
        var expectedZipCode = "10001";

        var command = new StoreCreateCommand(authInfo)
        {
            Name = expectedStoreName,
            Phone = expectedPhone,
            Email = expectedEmail,
            Street = expectedStreet,
            City = expectedCity,
            State = expectedState,
            ZipCode = expectedZipCode
        };

        using var context = await _factory.Create();
        var storeId = await context.Execute(new StoreCreateCommandPersistence(command));

        Assert.That(storeId, Is.Not.EqualTo(Guid.Empty));
        Assert.That(storeId, Is.EqualTo(command.Id));
    }

    [Test]
    public async Task StoreCreateCommand_ShouldInsertStore_WithGuidIdAndQuery()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedStoreName = $"Store_{Random.Shared.NextSingle()}";
        var expectedPhone = "(555) 123-4567";
        var expectedEmail = "store@example.com";

        var command = new StoreCreateCommand(authInfo)
        {
            Name = expectedStoreName,
            Phone = expectedPhone,
            Email = expectedEmail,
            Street = "456 Oak Ave",
            City = "Los Angeles",
            State = "CA",
            ZipCode = "90001"
        };

        using var context = await _factory.Create(new MSSQLTransaction());
        var storeId = await context.Execute(new StoreCreateCommandPersistence(command));
        Assert.That(storeId, Is.Not.EqualTo(Guid.Empty));
        context.Commit();

        var store = await context.Query(new StoreGetByIdQueryPersistence(new StoreGetByIdQuery(authInfo)
        {
            Id = storeId.Value
        }));

        Assert.That(store, Is.Not.Null);
        Assert.That(store!.Name, Is.EqualTo(expectedStoreName));
        Assert.That(store.Phone, Is.EqualTo(expectedPhone));
        Assert.That(store.Email, Is.EqualTo(expectedEmail));
        Assert.That(store.Id, Is.EqualTo(storeId.Value));
    }
}