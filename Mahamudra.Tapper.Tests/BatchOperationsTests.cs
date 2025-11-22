using Mahamudra.Tapper.Interfaces;
using Mahamudra.Tapper.Tests.Brands.Commands;
using Mahamudra.Tapper.Tests.Common;
using Mahamudra.Tapper.Tests.MSSQL;
using System.Data;

namespace Mahamudra.Tapper.Tests
{
    public class BatchOperationsTests
    {
        private IDbContextFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = ServicesProvider.GetRequiredService<IProductionDbContextFactory>();
        }

        [Test]
        public async Task ExecuteBatchAsync_ShouldInsertMultipleRecords()
        {
            var authInfo = new AuthenticationInfo { Id = Guid.NewGuid().ToString() };
            var brands = new List<BrandCreateCommand>();
            for (int i = 0; i < 5; i++)
            {
                brands.Add(new BrandCreateCommand(authInfo)
                {
                    Name = $"BatchBrand_{Guid.NewGuid()}"
                });
            }

            using var context = await _factory.Create(new MSSQLTransaction());
            
            var commandPersistence = new BrandBatchCreateCommandPersistence(brands);
            var rowsAffected = await context.Execute(commandPersistence);

            Assert.That(rowsAffected, Is.EqualTo(5));
            context.Commit();
        }

        private sealed class BrandBatchCreateCommandPersistence : DapperBase, ICommand<int>
        {
            private readonly IEnumerable<BrandCreateCommand> _commands;
            private static readonly string _sqlInsert = @"
INSERT INTO /*schema*/ brands 
            (brand_name)
VALUES      (@Name);"; // Note: @Name matches the property in BrandCreateCommand

            public BrandBatchCreateCommandPersistence(IEnumerable<BrandCreateCommand> commands)
            {
                _commands = commands;
            }

            public async Task<int> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string schema = null)
            {
                // We use the new ExecuteBatchAsync method here
                return await ExecuteBatchAsync(connection, _sqlInsert.Add(schema), _commands, transaction, CommandType.Text);
            }
        }
    }
}
