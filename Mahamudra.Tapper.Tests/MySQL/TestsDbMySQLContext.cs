using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
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

    [Test]
    public async Task ProductCreateAndQuery_BufferedVsUnbuffered_InTransaction()
    {
        var authInfo = BasicAuthenticationInfo;
        var testResults = new System.Text.StringBuilder();
        testResults.AppendLine("\n╔════════════════════════════════════════════════════════════════════════════════════╗");
        testResults.AppendLine("║                        Buffered vs Unbuffered Test Results                        ║");
        testResults.AppendLine("╠════════════╦═══════════╦════════════╦═════════════╦═══════════════╦══════════════╣");
        testResults.AppendLine("║ Test Mode  ║ ProductID ║ Category   ║ Buffered    ║ In Transaction║ Result       ║");
        testResults.AppendLine("╠════════════╬═══════════╬════════════╬═════════════╬═══════════════╬══════════════╣");

        // Test 1: Buffered mode in transaction
        try
        {
            using var context1 = await _factory.Create(new MySQLTransaction());

            var categoryId1 = await context1.Execute(new CategoryMySqlCreateCommandPersistence(
                new CategoryCreateCommand(authInfo) { Name = "Cat_Buffered" }));

            var brandId1 = await context1.Execute(new BrandMySqlCreateCommandPersistence(
                new BrandCreateCommand(authInfo) { Name = "Brand_Buffered" }));

            var productId1 = await context1.Execute(new ProductMySqlCreateCommandPersistence(
                new ProductCreateCommand(authInfo)
                {
                    Name = "Product_Buffered",
                    ListPrice = 100M,
                    ModelYear = 2025,
                    BrandId = brandId1.Value,
                    CategoryId = categoryId1.Value
                }));

            var product1 = await context1.Query(new ProductCategoryGetByIdQueryPersistence(
                new ProductGetByIdQuery(authInfo) { Id = productId1.Value }));

            testResults.AppendLine($"║ Buffered   ║ {productId1.Value,-9} ║ {product1?.Category?.Name,-10} ║ true        ║ Yes           ║ ✓ PASS       ║");

            context1.Commit();
        }
        catch (Exception ex)
        {
            testResults.AppendLine($"║ Buffered   ║ N/A       ║ N/A        ║ true        ║ Yes           ║ ✗ FAIL       ║");
            testResults.AppendLine($"║            Error: {ex.Message.Substring(0, Math.Min(60, ex.Message.Length)),-60}║");
        }

        // Test 2: Unbuffered mode in transaction
        try
        {
            using var context2 = await _factory.Create(new MySQLTransaction());

            var categoryId2 = await context2.Execute(new CategoryMySqlCreateCommandPersistence(
                new CategoryCreateCommand(authInfo) { Name = "Cat_Unbuf" }));

            var brandId2 = await context2.Execute(new BrandMySqlCreateCommandPersistence(
                new BrandCreateCommand(authInfo) { Name = "Brand_Unbuf" }));

            var productId2 = await context2.Execute(new ProductMySqlCreateCommandPersistence(
                new ProductCreateCommand(authInfo)
                {
                    Name = "Product_Unbuf",
                    ListPrice = 200M,
                    ModelYear = 2025,
                    BrandId = brandId2.Value,
                    CategoryId = categoryId2.Value
                }));

            var product2 = await context2.Query(new ProductCategoryGetByIdQueryUnbufferedPersistence(
                new ProductGetByIdQuery(authInfo) { Id = productId2.Value }));

            var categoryName2 = product2?.Category?.Name ?? "N/A";
            testResults.AppendLine($"║ Unbuffered ║ {productId2.Value,-9} ║ {categoryName2,-10} ║ false       ║ Yes           ║ ✓ PASS       ║");

            context2.Commit();
        }
        catch (Exception ex)
        {
            testResults.AppendLine($"║ Unbuffered ║ N/A       ║ N/A        ║ false       ║ Yes           ║ ✗ FAIL       ║");
            testResults.AppendLine($"║            Error: {ex.Message.Substring(0, Math.Min(60, ex.Message.Length)),-60}║");
        }

        // Test 3: Create in transaction, query unbuffered AFTER commit
        int? productId3 = null;
        try
        {
            using (var context3 = await _factory.Create(new MySQLTransaction()))
            {
                var categoryId3 = await context3.Execute(new CategoryMySqlCreateCommandPersistence(
                    new CategoryCreateCommand(authInfo) { Name = "Cat_AfterCommit" }));

                var brandId3 = await context3.Execute(new BrandMySqlCreateCommandPersistence(
                    new BrandCreateCommand(authInfo) { Name = "Brand_AfterCommit" }));

                productId3 = await context3.Execute(new ProductMySqlCreateCommandPersistence(
                    new ProductCreateCommand(authInfo)
                    {
                        Name = "Product_AfterCommit",
                        ListPrice = 300M,
                        ModelYear = 2025,
                        BrandId = brandId3.Value,
                        CategoryId = categoryId3.Value
                    }));

                context3.Commit();
            }

            // Query after commit in new context
            using (var queryContext = await _factory.Create())
            {
                var product3 = await queryContext.Query(new ProductCategoryGetByIdQueryUnbufferedPersistence(
                    new ProductGetByIdQuery(authInfo) { Id = productId3.Value }));

                var categoryName3 = product3?.Category?.Name ?? "N/A";
                testResults.AppendLine($"║ Unbuffered ║ {productId3.Value,-9} ║ {categoryName3,-10} ║ false       ║ No (committed)║ ✓ PASS       ║");
            }
        }
        catch (Exception ex)
        {
            testResults.AppendLine($"║ Unbuffered ║ {productId3?.ToString() ?? "N/A",-9} ║ N/A        ║ false       ║ No (committed)║ ✗ FAIL       ║");
            testResults.AppendLine($"║            Error: {ex.Message.Substring(0, Math.Min(60, ex.Message.Length)),-60}║");
        }

        testResults.AppendLine("╚════════════╩═══════════╩════════════╩═════════════╩═══════════════╩══════════════╝");

        Console.WriteLine(testResults.ToString());
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
        var storeId = await context.Execute(new StoreMySqlCreateCommandPersistence(command));

        Assert.That(storeId, Is.Not.EqualTo(Guid.Empty));
        Assert.That(storeId, Is.EqualTo(command.Id));
    }

    [Test]
    public async Task StoreCreateCommand_ShouldInsertStore_WithGuidIdAndQuery()
    {
        var authInfo = BasicAuthenticationInfo;
        var expectedStoreName = $"Store_{Random.Shared.NextSingle()}";
        var expectedPhone = "(555) 987-6543";
        var expectedEmail = "mysqlstore@example.com";

        var command = new StoreCreateCommand(authInfo)
        {
            Name = expectedStoreName,
            Phone = expectedPhone,
            Email = expectedEmail,
            Street = "789 Pine Blvd",
            City = "Chicago",
            State = "IL",
            ZipCode = "60601"
        };

        using var context = await _factory.Create(new MySQLTransaction());
        var storeId = await context.Execute(new StoreMySqlCreateCommandPersistence(command));
        Assert.That(storeId, Is.Not.EqualTo(Guid.Empty));
        context.Commit();

        var store = await context.Query(new StoreMySqlGetByIdQueryPersistence(new StoreGetByIdQuery(authInfo)
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