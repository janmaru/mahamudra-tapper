using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Commands;
public sealed class ProductCreateCommand(IAuthenticationInfo authenticationInfo) 
{
    public IAuthenticationInfo AuthenticationInfo { get; internal set; } = authenticationInfo; 
    public string? Name { get; set; }
    public int BrandId { get; set; } 
    public int CategoryId { get; set; } 
    public Int16 ModelYear { get; set; } 
    public decimal ListPrice { get; set; }
}