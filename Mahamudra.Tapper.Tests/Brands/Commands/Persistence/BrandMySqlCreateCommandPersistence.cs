using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Brands.Commands;
using Mahamudra.Tapper.Tests.Common;
using System.Data;

namespace Mahamudra.Tapper.Tests.Brands.Commands.Persistence
{

    public sealed class BrandMySqlCreateCommandPersistence  : DapperBase, ICommand<int?>
    {
        public BrandMySqlCreateCommandPersistence(BrandCreateCommand command)
        {
            this._command = command;
        }

        private readonly BrandCreateCommand _command;

        private static readonly string _sqlInsert = @"
INSERT INTO /*schema*/ brands 
            (brand_name)
VALUES      (@Name); 
Select LAST_INSERT_ID()";

        public async Task<int?> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
                  => await ((IPersistence)this).ExecuteAsync<int?>(connection!, _sqlInsert.Add(schema), _command, transaction);
    }
}