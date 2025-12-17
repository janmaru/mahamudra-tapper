using Mahamudra.Tapper.Tests.Brands;
using Mahamudra.Tapper.Tests.Common;
using Mediator;

namespace Mahamudra.Tapper.Tests.Brands.Commands;
public sealed class BrandCreateCommand : IRequest<Brand>
{
    public BrandCreateCommand(IAuthenticationInfo authenticationInfo)
    {
        this.AuthenticationInfo = authenticationInfo;   
    }

    public IAuthenticationInfo AuthenticationInfo { get; internal set; } 
    public string? Name { get; set; } 
}