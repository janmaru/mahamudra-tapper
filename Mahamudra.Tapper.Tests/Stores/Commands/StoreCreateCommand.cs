using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Stores;
using Mediator;

namespace Mahamudra.Tapper.Tests.Stores.Commands;

public sealed class StoreCreateCommand : IRequest<Store>
{
    public StoreCreateCommand(IAuthenticationInfo authenticationInfo)
    {
        this.AuthenticationInfo = authenticationInfo;
        // Generate a new GUID for the store
        this.Id = Guid.NewGuid();
    }

    public IAuthenticationInfo AuthenticationInfo { get; internal set; }
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}
