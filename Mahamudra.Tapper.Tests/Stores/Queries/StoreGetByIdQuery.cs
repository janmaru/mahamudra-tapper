using Mahamudra.Tapper.Tests.Common;

namespace Mahamudra.Tapper.Tests.Stores.Queries;

public class StoreGetByIdQuery
{
    public StoreGetByIdQuery(IAuthenticationInfo authenticationInfo)
    {
        this.AuthenticationInfo = authenticationInfo;
    }

    public IAuthenticationInfo AuthenticationInfo { get; internal set; }
    public int Id { get; set; }
}
