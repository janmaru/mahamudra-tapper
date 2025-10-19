using System.Data;
using System.Threading.Tasks;
using System.Threading;

namespace Mahamudra.Tapper.Interfaces
{ 
    public interface ICommand<T>
    {
        Task<T> Execute(IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default, string schema = null);
    }
}