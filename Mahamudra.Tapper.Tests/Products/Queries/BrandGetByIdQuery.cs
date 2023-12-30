using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Products.Queries;

public class BrandGetByIdQuery(IAuthenticationInfo authenticationInfo)
{
    public IAuthenticationInfo AuthenticationInfo { get; internal set; } = authenticationInfo;
    public int Id { get; set; }
} 