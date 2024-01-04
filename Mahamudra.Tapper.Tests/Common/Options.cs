namespace Mahamudra.Tapper.Tests.Common;

public class Options
{
    public MSSQL? MSSQL { get; set; }
    public MySQL? MySQL { get; set; }
}

public abstract class Database 
{

    public string? SchemaProduction { get; set; } 
    public string? SchemaSales { get; set; }
}  

public class MSSQL: Database
{
    public string? ConnectionString { get; set; }
}

public class MySQL : Database
{
    public string? ConnectionStringProduction { get; set; }
    public string? ConnectionStringSales { get; set; }
}