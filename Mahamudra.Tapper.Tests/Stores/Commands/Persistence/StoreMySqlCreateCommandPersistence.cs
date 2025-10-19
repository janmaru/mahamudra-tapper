using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using System.Data;
using Mahamudra.Tapper.Tests.Stores.Commands;

namespace Mahamudra.Tapper.Tests.Stores.Commands.Persistence
{
    /// <summary>
    /// MySQL persistence for Store creation with GUID ID (no auto-increment)
    /// </summary>
    public sealed class StoreMySqlCreateCommandPersistence : DapperBase, ICommand<Guid?>
    {
        public StoreMySqlCreateCommandPersistence(StoreCreateCommand command)
        {
            this._command = command;
        }

        private readonly StoreCreateCommand _command;

        // MySQL: No LAST_INSERT_ID() needed since we're providing the GUID
        // Note: MySQL stores GUID as CHAR(36) or BINARY(16)
        private static readonly string _sqlInsert = @"
INSERT INTO /*schema*/ stores
            (store_id, store_name, phone, email, street, city, state, zip_code)
VALUES      (@Id, @Name, @Phone, @Email, @Street, @City, @State, @ZipCode);
SELECT @Id";

        public async Task<Guid?> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
                  => await ((IPersistence)this).ExecuteAsync<Guid?>(connection!, _sqlInsert.Add(schema), _command, transaction);
    }
}
