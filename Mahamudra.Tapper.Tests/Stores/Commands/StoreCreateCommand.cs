using Mahamudra.Tapper.Tests.Common;
using Mediator;

namespace Mahamudra.Tapper.Tests.Stores.Commands;

public sealed class StoreCreateCommand : IRequest<Store>
{
    public StoreCreateCommand(IAuthenticationInfo authenticationInfo)
    {
        this.AuthenticationInfo = authenticationInfo;
    }

    public IAuthenticationInfo AuthenticationInfo { get; internal set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}
