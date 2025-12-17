using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Brands.Queries;

public class BrandGetByIdQuery 
{
    public BrandGetByIdQuery(IAuthenticationInfo authenticationInfo)
    {
        this.AuthenticationInfo = authenticationInfo;
    }

    public IAuthenticationInfo AuthenticationInfo { get; internal set; }  
    public int Id { get; set; }
} 