namespace Mahamudra.Tapper.Tests.Common;

public class Options
{
    public MSSQL? MSSQL { get; set; }
}

public class MSSQL
{
    public string? ConnectionString { get; set; }
    public string? SchemaProduction { get; set; } 
    public string? SchemaSales { get; set; }
}  