using Mahamudra.Tapper.Tests.Brands;
using Mahamudra.Tapper.Tests.Brands.Commands;
using Mahamudra.Tapper.Tests.Brands.Commands.Persistence;
using Mahamudra.Tapper.Tests.MSSQL;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Mahamudra.Tapper.Tests.Brands.CommandHandlers;

public class BrandCreateCommandHandler : IRequestHandler<BrandCreateCommand, Brand>
{
    private readonly IProductionDbContextFactory _factory;
    private readonly ILogger<BrandCreateCommandHandler> _logger;

    public BrandCreateCommandHandler(
     IProductionDbContextFactory factory,
     ILogger<BrandCreateCommandHandler> logger)
    {
        this._factory = factory;
        this._logger = logger;
    }

    public async ValueTask<Brand> Handle(BrandCreateCommand command, CancellationToken ct)
    {
        // Create brand using factory method (validation happens inside)
        var brand = Brand.Create(command.Name!);

        using var context = await _factory.Create(ct: ct);
        var brandId = await context.Execute(new BrandCreateCommandPersistence(
           new BrandCreateCommand(command.AuthenticationInfo)
           {
               Name = brand.Name
           }));

        // Reconstitute with database-generated ID
        return Brand.Reconstitute(brandId.Value, brand.Name);
    }
}