using Mahamudra.Tapper.Exceptions;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.MSSQL;
using Mahamudra.Tapper.Tests.MySQL;
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

        services.AddSingleton<IProductionDbContextFactory>(new DbMSSQLContextFactor(mSSQLConnectionString, options!.MSSQL!.SchemaProduction!));
        services.AddSingleton<ISalesDbContextFactory>(new DbMSSQLContextFactor(mSSQLConnectionString, options!.MSSQL!.SchemaSales!));

        // Microsoft SQL Server --- end

        // MySQL ---start
        var mySQLConnectionStringProduction = options!.MySQL!.ConnectionStringProduction ??
                      throw new TapperException("MySQL Db connection string production not configured");
        var mySQLConnectionStringSales = options!.MySQL!.ConnectionStringSales ??
              throw new TapperException("MySQL Db connection string sales not configured");
        services.AddSingleton<IProductionMySQLDbContextFactory>(new DbMySQLContextFactory(mySQLConnectionStringProduction, options!.MySQL!.SchemaProduction!));
        services.AddSingleton<ISalesMySQLDbContextFactory>(new DbMySQLContextFactory(mySQLConnectionStringSales, options!.MySQL!.SchemaSales!));
        // MySQL --- end

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