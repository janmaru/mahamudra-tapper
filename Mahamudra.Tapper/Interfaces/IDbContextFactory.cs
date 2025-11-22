using System.Threading;
using System.Threading.Tasks;

namespace Mahamudra.Tapper.Interfaces
{
    public interface IDbContextFactory
    {
        Task<IDbContext> Create(ITransaction transactional = null, CancellationToken ct = default);
    }
}