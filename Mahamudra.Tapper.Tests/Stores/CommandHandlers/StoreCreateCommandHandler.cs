using Mahamudra.Tapper.Tests.Stores;
using Mahamudra.Tapper.Tests.Stores.Commands;
using Mahamudra.Tapper.Tests.Stores.Commands.Persistence;
using Mahamudra.Tapper.Tests.MSSQL;
using Mahamudra.Tapper.Tests.Products.Commands;
using Mahamudra.Tapper.Tests.Products.Commands.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Mahamudra.Tapper.Tests.Stores.CommandHandlers;

public class StoreCreateCommandHandler : IRequestHandler<StoreCreateCommand, Store>
{
    private readonly IProductionDbContextFactory _factory;
    private readonly ILogger<StoreCreateCommandHandler> _logger;

    public StoreCreateCommandHandler(
     IProductionDbContextFactory factory,
     ILogger<StoreCreateCommandHandler> logger)
    {
        this._factory = factory;
        this._logger = logger;
    }

    public async ValueTask<Store> Handle(StoreCreateCommand command, CancellationToken ct)
    {
        // Create store using factory method (validation happens inside)
        var store = Store.Create(
            command.Name!,
            command.Phone,
            command.Email,
            command.Street,
            command.City,
            command.State,
            command.ZipCode);

        // Set the ID from command if provided, otherwise use generated one
        if (command.Id != Guid.Empty)
        {
            // Use Reconstitute to set specific ID
            store = Store.Reconstitute(
                command.Id,
                store.Name,
                store.Phone,
                store.Email,
                store.Street,
                store.City,
                store.State,
                store.ZipCode);
        }

        using var context = await _factory.Create(ct: ct);
        var storeId = await context.Execute(new StoreCreateCommandPersistence(
           new StoreCreateCommand(command.AuthenticationInfo)
           {
               Id = store.Id,
               Name = store.Name,
               Phone = store.Phone,
               Email = store.Email,
               Street = store.Street,
               City = store.City,
               State = store.State,
               ZipCode = store.ZipCode
           }));

        return Store.Reconstitute(
            storeId!.Value,
            store.Name,
            store.Phone,
            store.Email,
            store.Street,
            store.City,
            store.State,
            store.ZipCode);
    }
}
