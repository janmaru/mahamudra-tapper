﻿using Mahamudra.Tapper.Exceptions;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.MSSQL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mahamudra.Tapper.Tests;

public static class ServicesProvider
{
    private static IServiceProvider Provider()
    {
        var services = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddSingleton<IConfiguration>(configurationBuilder);

        var configuration = services
            .BuildServiceProvider()
            .GetRequiredService<IConfiguration>();

        Options options = new();
        configuration.GetSection(nameof(Options)).Bind(options);
        services.AddSingleton(options);


        // Microsoft SQL Server ---start
        var mSSQLConnectionString = options!.MSSQL!.ConnectionString ??
                      throw new TapperException("MSSQL Db connection string not configured"); 
        services.AddSingleton<IProductionDbContextFactory>(new DbMSSQLContextFactory(mSSQLConnectionString, options!.MSSQL!.SchemaProduction!));
        services.AddSingleton<ISalesDbContextFactory>(new DbMSSQLContextFactory(mSSQLConnectionString, options!.MSSQL!.SchemaSales!));
        //Microsoft SQL Server --- end

        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(RootInfrastructure).Assembly));

        services.AddLogging();

        return services.BuildServiceProvider();
    }

    internal static T GetRequiredService<T>() where T : class
    {
        var provider = Provider();
        return provider.GetRequiredService<T>();
    }
}