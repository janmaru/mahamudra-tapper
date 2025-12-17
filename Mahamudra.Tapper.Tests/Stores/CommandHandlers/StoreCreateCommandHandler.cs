using Mahamudra.Tapper.Tests.MSSQL;
using Mahamudra.Tapper.Tests.Stores.Commands;
using Mahamudra.Tapper.Tests.Stores.Commands.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Mahamudra.Tapper.Tests.Stores.CommandHandlers;

public class StoreCreateCommandHandler : IRequestHandler<StoreCreateCommand, Store>
{
    private readonly ISalesDbContextFactory _factory;
    private readonly ILogger<StoreCreateCommandHandler> _logger;

    public StoreCreateCommandHandler(
     ISalesDbContextFactory factory,
     ILogger<StoreCreateCommandHandler> logger)
    {
        this._factory = factory;
        this._logger = logger;
    }

    public async ValueTask<Store> Handle(StoreCreateCommand command, CancellationToken ct)
    {
        using var context = await _factory.Create(ct: ct);
        var storeId = await context.Execute(new StoreCreateCommandPersistence(command));

        return Store.Reconstitute(
            storeId!.Value,
            command.Name!,
            command.Phone,
            command.Email,
            command.Street,
            command.City,
            command.State,
            command.ZipCode);
    }
}
