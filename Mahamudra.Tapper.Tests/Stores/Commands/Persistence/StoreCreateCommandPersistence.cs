using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.Stores.Commands;
using System.Data;

namespace Mahamudra.Tapper.Tests.Stores.Commands.Persistence
{
    /// <summary>
    /// MSSQL persistence for Store creation with GUID ID (no identity column)
    /// </summary>
    public sealed class StoreCreateCommandPersistence : DapperBase, ICommand<Guid?>
    {
        public StoreCreateCommandPersistence(StoreCreateCommand command)
        {
            this._command = command;
        }

        private readonly StoreCreateCommand _command;

        // MSSQL: No SCOPE_IDENTITY() needed since we're providing the GUID
        private static readonly string _sqlInsert = @"
INSERT INTO /*schema*/ stores
            (store_id, store_name, phone, email, street, city, state, zip_code)
VALUES      (@Id, @Name, @Phone, @Email, @Street, @City, @State, @ZipCode);
SELECT @Id";

        public async Task<Guid?> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
                  => await ((IPersistence)this).ExecuteAsync<Guid?>(connection!, _sqlInsert.Add(schema), _command, transaction);
    }
}
