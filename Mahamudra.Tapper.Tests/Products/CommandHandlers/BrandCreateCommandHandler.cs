using Mahamudra.Tapper.Tests.MSSQL;
using Mahamudra.Tapper.Tests.Products.Commands;
using Mahamudra.Tapper.Tests.Products.Commands.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mahamudra.Tapper.Tests.Products.CommandHandlers;

public class BrandCreateCommandHandler(
     IProductionDbContextFactory factory,
     ILogger<BrandCreateCommandHandler> logger) : IRequestHandler<BrandCreateCommand, Brand>
{
    private readonly IProductionDbContextFactory _factory = factory;
    private readonly ILogger<BrandCreateCommandHandler> _logger = logger;

    public async Task<Brand> Handle(BrandCreateCommand command, CancellationToken ct)
    {
        //validation
        if (string.IsNullOrEmpty(command.Name) || command.Name.Length > 255)
            throw new ArgumentException("Name should bla bla...");

        using var context = await _factory.Create(ct: ct);
        var brandId = await context.Execute(new BrandCreateCommandPersistence(
           new BrandCreateCommand(command.AuthenticationInfo)
           {
               Name = command.Name
           }));
        return new Brand()
        {
            Name = command.Name,
            Id = brandId.Value
        };
    }
}