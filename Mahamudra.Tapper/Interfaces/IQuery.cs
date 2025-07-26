using System.Data;
using System.Threading;
using System.Threading.Tasks;
namespace Mahamudra.Tapper.Interfaces
{
    public interface IQuery<T>
    {
        Task<T> Select(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string schema = null);
    }
}