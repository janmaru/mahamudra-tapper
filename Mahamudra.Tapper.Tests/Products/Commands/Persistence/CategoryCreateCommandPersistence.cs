using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Commands.Persistence
{

    public sealed class CategoryCreateCommandPersistence : DapperBase, ICommand<int?>
    {
        public CategoryCreateCommandPersistence(CategoryCreateCommand command)
        {
            this._command = command;
        }

        private readonly CategoryCreateCommand _command;

        private static readonly string _sqlInsert = @"
INSERT INTO /*schema*/ categories 
            (category_name)
VALUES      (@name); 
Select SCOPE_IDENTITY()";

        public async Task<int?> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
                  => await ((IPersistence)this).ExecuteAsync<int?>(connection!, _sqlInsert.Add(schema), _command, transaction);
    }
}