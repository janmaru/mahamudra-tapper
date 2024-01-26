using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Commands;
public sealed class CategoryCreateCommand 
{
    public CategoryCreateCommand(IAuthenticationInfo authenticationInfo)
    {
        this.AuthenticationInfo = authenticationInfo;
    }

    public IAuthenticationInfo AuthenticationInfo { get; internal set; }
    public string? Name { get; set; } 
}