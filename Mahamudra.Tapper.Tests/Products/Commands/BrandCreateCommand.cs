using Mahamudra.Tapper.Tests.Common;
using MediatR;

namespace Mahamudra.Tapper.Tests.Products.Commands;
public sealed class BrandCreateCommand(IAuthenticationInfo authenticationInfo) : IRequest<Brand>
{
    public IAuthenticationInfo AuthenticationInfo { get; internal set; } = authenticationInfo; 
    public string? Name { get; set; } 
}