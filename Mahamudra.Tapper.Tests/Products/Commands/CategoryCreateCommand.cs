using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Commands;
public sealed class CategoryCreateCommand(IAuthenticationInfo authenticationInfo) 
{
    public IAuthenticationInfo AuthenticationInfo { get; internal set; } = authenticationInfo; 
    public string? Name { get; set; } 
}