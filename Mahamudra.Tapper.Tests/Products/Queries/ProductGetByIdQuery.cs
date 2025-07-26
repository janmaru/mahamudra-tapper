using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Queries;

public class ProductGetByIdQuery 
{
    public ProductGetByIdQuery(IAuthenticationInfo authenticationInfo)
    {
        this.AuthenticationInfo = authenticationInfo;
    }
    public IAuthenticationInfo AuthenticationInfo { get; internal set; } 
    public int Id { get; set; }
} 