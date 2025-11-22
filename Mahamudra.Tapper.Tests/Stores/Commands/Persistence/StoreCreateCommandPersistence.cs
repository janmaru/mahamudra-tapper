using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using System.Data;

namespace Mahamudra.Tapper.Tests.Stores.Commands.Persistence
{
    /// <summary>
    /// MSSQL persistence for Store creation with INT identity column
    /// </summary>
    public sealed class StoreCreateCommandPersistence : DapperBase, ICommand<int?>
    {
        public StoreCreateCommandPersistence(StoreCreateCommand command)
        {
            this._command = command;
        }

        private readonly StoreCreateCommand _command;

        private static readonly string _sqlInsert = @"
INSERT INTO /*schema*/ stores
            (store_name, phone, email, street, city, state, zip_code)
VALUES      (@Name, @Phone, @Email, @Street, @City, @State, @ZipCode);
SELECT CAST(SCOPE_IDENTITY() as int)";

        public async Task<int?> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
                  => await ((IPersistence)this).ExecuteAsync<int?>(connection!, _sqlInsert.Add(schema), _command, transaction);
    }
}
