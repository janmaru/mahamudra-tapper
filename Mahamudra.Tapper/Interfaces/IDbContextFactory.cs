using System.Data;
using System.Threading.Tasks;
using System.Threading;

namespace Mahamudra.Tapper.Interfaces
{
    public interface IDbContextFactory
    {
        Task<IDbContext> Create(ITransaction transactional = null, CancellationToken ct = default);
    }
}