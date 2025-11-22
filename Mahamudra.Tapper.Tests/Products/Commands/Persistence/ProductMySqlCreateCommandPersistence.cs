using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Common;
using System.Data;

namespace Mahamudra.Tapper.Tests.Products.Commands.Persistence
{

    public sealed class ProductMySqlCreateCommandPersistence : DapperBase, ICommand<int?>
    {
        public ProductMySqlCreateCommandPersistence(ProductCreateCommand command)
        {
            this._command = command;
        }

        private readonly ProductCreateCommand _command;

        private static readonly string _sqlInsert = @"
INSERT INTO /*schema*/ products 
            (product_name,
             brand_id,
             category_id,
             model_year,
             list_price)
VALUES     (@Name,
            @BrandId,
            @CategoryId,
            @ModelYear,
            @ListPrice); 
Select LAST_INSERT_ID()";

        public async Task<int?> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string? schema = null)
                  => await ((IPersistence)this).ExecuteAsync<int?>(connection!, _sqlInsert.Add(schema), _command, transaction);
    }
}