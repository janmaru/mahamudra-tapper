﻿using Mahamudra.Tapper.Tests.Common;
using MediatR;

namespace Mahamudra.Tapper.Tests.Products.Commands;
public sealed class BrandCreateCommand : IRequest<Brand>
{
    public BrandCreateCommand(IAuthenticationInfo authenticationInfo)
    {
        this.AuthenticationInfo = authenticationInfo;   
    }

    public IAuthenticationInfo AuthenticationInfo { get; internal set; } 
    public string? Name { get; set; } 
}