using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Products.Commands;
using Mahamudra.Tapper.Tests.Products.Commands.Persistence;
using Mahamudra.Tapper.Tests.Products.Queries;
using Mahamudra.Tapper.Tests.Products.Queries.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mahamudra.Tapper.Tests.MSSQL;

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
}